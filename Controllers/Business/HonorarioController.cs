using Calcular.CoreApi.BusinessObjects;
using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Models.ViewModels;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class HonorarioController : Controller
    {
        private readonly ApplicationDbContext db;

        public HonorarioController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetPaged([FromQuery] string filter, int itemsPerPage, int page = 1)
        {
            var comissoes = db.Comissoes.ToList();

            var pendente = filter.ContainsIgnoreNonSpacing("pendente");
            var pago = filter.ContainsIgnoreNonSpacing("pago");
            var atrasado = filter.ContainsIgnoreNonSpacing("atrasado");

            var query = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.Servicos).ThenInclude(x => x.Atividades).ThenInclude(x => x.ComissaoAtividade)
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || (pago && x.StatusHonorario == StatusHonorarioEnum.Pago)
                                        || (pendente  && x.StatusHonorario == StatusHonorarioEnum.Pendente)
                                        || (atrasado && x.StatusHonorario == StatusHonorarioEnum.Pendente && x.PrazoHonorario.HasValue && x.PrazoHonorario.Value.Date < DateTime.Now.Date)
                                        || x.Numero.Contains(filter)
                                        || x.Reu.Contains(filter)
                                        || x.Autor.Contains(filter)
                                        || x.Advogado.Nome.Contains(filter)
                                        || x.Advogado.Empresa.Contains(filter));

            var table = query.OrderBy(x => x.Id)
                            .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                            .ToList()
                            .Select(x => {
                                var valorComissao = x.Servicos.Sum(s => s.Atividades.Sum(a => a.Valor ?? (a.ComissaoAtividade != null ? a.ComissaoAtividade.ValorFinal : null) ?? ApuracaoComissaoBO.GetValorBase(comissoes, a)));
                                return new ProcessoViewModel
                                {
                                    Numero = x.Numero,
                                    Id = x.Id,
                                    Autor = x.Autor,
                                    Reu = x.Reu,
                                    Honorario = x.Honorario,
                                    Prazo = x.PrazoHonorario,
                                    Total = x.Total,
                                    CustoComissao = valorComissao > 0 ? (decimal?)valorComissao : null,
                                    Parte = x.Parte,
                                    Local = x.Local,
                                    NumeroAutores = x.NumeroAutores,
                                    AdvogadoId = x.AdvogadoId,
                                    Vara = x.Vara,
                                    Perito = x.Perito,
                                    Indicacao = x.Indicacao,
                                    StatusHonorario = (x.StatusHonorario == StatusHonorarioEnum.Pago || x.StatusHonorario == StatusHonorarioEnum.Undefined) ? EnumHelpers.GetEnumDescription(x.StatusHonorario)
                                                        : x.StatusHonorario == StatusHonorarioEnum.Pendente && x.PrazoHonorario.HasValue && x.PrazoHonorario.Value.Date < DateTime.Now.Date ? EnumHelpers.GetEnumDescription(StatusHonorarioEnum.Atrasado)
                                                        : EnumHelpers.GetEnumDescription(StatusHonorarioEnum.Pendente),
                                    Advogado = new Cliente
                                    {
                                        Id = x.Advogado.Id,
                                        Nome = x.Advogado.Nome,
                                        Empresa = x.Advogado.Empresa,
                                    },
                                };
                            });

            return Ok(new { data = table, totalItems = query.Count() });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Honorarios.SingleOrDefault(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Honorario honorario)
        {
            try
            {
                db.Honorarios.Add(honorario);
                db.SaveChanges();
                SetHonorarioStatus(honorario.ProcessoId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(honorario);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Honorarios.Single(x => x.Id == id);
            item.Cancelado = true;
            db.SaveChanges();

            SetHonorarioStatus(item.ProcessoId);

            return Ok(item);
        }

        [Route("registro")]
        [HttpGet]
        public IActionResult GetLocal()
        {
            var result = Enum.GetValues(typeof(RegistroEnum))
                            .Cast<RegistroEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [Route("tipopagamento/{registro}")]
        [HttpGet]
        public IActionResult GetParte(RegistroEnum registro)
        {
            switch (registro)
            {
                case RegistroEnum.Honorario:
                    return Ok(Enum.GetValues(typeof(TipoRegistroEnum))
                                 .Cast<TipoRegistroEnum>()
                                 .Where(x => (int)x <= 3)
                                 .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x))));
                case RegistroEnum.Pagamento:
                    return Ok(Enum.GetValues(typeof(TipoRegistroEnum))
                                .Cast<TipoRegistroEnum>()
                                 .Where(x => (int)x > 3)
                                .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x))));
                default:
                    return BadRequest();
            }
        }

        /// <summary>
        /// Atualiza o status de pagamento de honorários do processo.
        /// </summary>
        /// <param name="honorario"></param>
        private void SetHonorarioStatus(int processoId)
        {
            var processo = db.Processos.Include(x => x.Honorarios).Single(x => x.Id == processoId);

            processo.PrazoHonorario = GetPrazo(processo);

            if (processo.Honorarios == null || processo.Honorarios.Count(x => !x.Cancelado) == 0)
                processo.StatusHonorario = StatusHonorarioEnum.Undefined;
            else if (processo.Honorario.HasValue && processo.Honorario == 0 && !processo.Honorarios.Any(x => !x.Cancelado && x.Registro == RegistroEnum.Pagamento))
                processo.StatusHonorario = StatusHonorarioEnum.Pendente;
            else if (processo.Total <= 0)
                processo.StatusHonorario = StatusHonorarioEnum.Pago;
            else
                processo.StatusHonorario = StatusHonorarioEnum.Pendente;

            db.SaveChanges();
        }

        private DateTime? GetPrazo(Processo processo)
        {
            if (processo.Honorarios != null && processo.Honorarios.Count > 0)
                return processo.Honorarios.Where(x => x.Registro == RegistroEnum.Honorario && !x.Cancelado).Max(x => x.Prazo);
            else
                return null;
        }
    }
}
