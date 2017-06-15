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
            var vazio = filter.ContainsIgnoreNonSpacing("vazio");

            var query = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.Servicos).ThenInclude(x => x.Atividades).ThenInclude(x => x.ComissaoAtividade)
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || pago || pendente || atrasado || vazio
                                        || x.Numero.Contains(filter)
                                        || x.Reu.Contains(filter)
                                        || x.Autor.Contains(filter)
                                        || x.Advogado.Nome.Contains(filter)
                                        || x.Advogado.Empresa.Contains(filter));

            // Se busca por status de honorário, concretiza a busca antes de filtrar.
            if (pago || pendente || atrasado || vazio)
            {
                query = query.ToList()
                        .Where(x => (pendente && x.StatusHonorario == "Pendente")
                                    || (pago && x.StatusHonorario == "Pago")
                                    || (atrasado && x.StatusHonorario == "Atrasado")
                                    || (vazio && x.StatusHonorario == "Vazio")).AsQueryable();
            }

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
                                    Prazo = x.Prazo,
                                    Total = x.Total,
                                    CustoComissao = valorComissao > 0 ? (decimal?)valorComissao : null,
                                    Parte = x.Parte,
                                    Local = x.Local,
                                    NumeroAutores = x.NumeroAutores,
                                    AdvogadoId = x.AdvogadoId,
                                    Vara = x.Vara,
                                    Perito = x.Perito,
                                    Indicacao = x.Indicacao,
                                    StatusHonorario = x.StatusHonorario,
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(honorario);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Honorario newItem)
        {
            var item = db.Honorarios.Single(x => x.Id == newItem.Id);

            item.NotaFiscal = newItem.NotaFiscal;
            item.Prazo = newItem.Prazo;
            item.Registro = newItem.Registro;
            item.TipoPagamento = newItem.TipoPagamento;
            item.ProcessoId = newItem.ProcessoId;
            item.Valor = newItem.Valor;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Honorarios.Single(x => x.Id == id);

            item.Cancelado = true;

            db.SaveChanges();
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
    }
}
