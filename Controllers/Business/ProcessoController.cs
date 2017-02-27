using Calcular.CoreApi.Common;
using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Models.ViewModels;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ProcessoController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public ProcessoController(UserManager<User> userManager, ApplicationDbContext db)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var query = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || x.Numero.Contains(filter)
                                        || x.Reu.Contains(filter)
                                        || x.Autor.Contains(filter)
                                        || x.Advogado.Nome.Contains(filter)
                                        || x.Advogado.Empresa.Contains(filter))
                            .OrderBy(x => x.Id)
                            .ToList();

            return Ok(query);
        }

        [HttpGet("paged")]
        public IActionResult GetPaged([FromQuery] string filter, int itemsPerPage, int page = 1)
        {
            var pendente = filter.ContainsIgnoreNonSpacing("pendente");
            var pago = filter.ContainsIgnoreNonSpacing("pago");
            var atrasado = filter.ContainsIgnoreNonSpacing("atrasado");

            var query = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Where(x => string.IsNullOrEmpty(filter) 
                                        || pago || pendente || atrasado
                                        || x.Numero.Contains(filter)
                                        || x.Reu.Contains(filter)
                                        || x.Autor.Contains(filter)
                                        || x.Advogado.Nome.Contains(filter)
                                        || x.Advogado.Empresa.Contains(filter));

            // Se busca por status de honorário, concretiza a busca antes de filtrar.
            if (pago || pendente || atrasado)
            {
                query = query.ToList()
                        .Where(x => (pendente && x.StatusHonorario == "Pendente")
                                    || (pago && x.StatusHonorario == "Pago")
                                    || (atrasado && x.StatusHonorario == "Atrasado")).AsQueryable();
            }

            var table = query.OrderBy(x => x.Id)
                            .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                            .ToList();

            return Ok(new { data = table, totalItems = query.Count() });
        }

        [HttpGet]
        [Route("numero")]
        public IActionResult GetByNumber([FromQuery] string filter)
        {
            if (filter.Length < 3)
                return BadRequest("Filtro deve conter no mínimo 3 caracteres.");

            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.ProcessoDetalhes).ThenInclude(x => x.User)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || x.Numero.Contains(filter))
                            .ToList()
                            .Select(x => new ProcessoViewModel
                            {
                                Numero = x.Numero,
                                Id = x.Id,
                                Autor = x.Autor,
                                Reu = x.Reu,
                                Honorario = x.Honorario,
                                Prazo = x.Prazo,
                                Total = x.Total,
                                Parte = x.Parte,
                                Local = x.Local,
                                NumeroAutores = x.NumeroAutores,
                                AdvogadoId = x.AdvogadoId,
                                Vara = x.Vara,
                                Perito = x.Perito,
                                Indicacao = x.Indicacao,
                                Advogado = new Cliente
                                {
                                    Id = x.Advogado.Id,
                                    Nome = x.Advogado.Nome
                                },
                                Honorarios = x.Honorarios.Select(h => new Honorario
                                {
                                    Id = h.Id,
                                    Data = h.Data,
                                    Registro = h.Registro,
                                    Valor = h.Valor,
                                    Prazo = h.Prazo,
                                    Observacao = h.Observacao,
                                    TipoPagamento = h.TipoPagamento,
                                    NotaFiscal = h.NotaFiscal,
                                }).ToList(),
                                ProcessoDetalhes = x.ProcessoDetalhes.Select(p => new ProcessoDetalhe
                                {
                                    Id = p.Id,
                                    Descricao = p.Descricao,
                                    Data = p.Data,
                                    User = new User
                                    {
                                        Id = p.User.Id,
                                        Name = p.User.Name
                                    }
                                }).ToList()
                            })
                            .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.ProcessoDetalhes).ThenInclude(x => x.User)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostProcesso([FromBody] Processo processo)
        {
            try
            {
                if (processo.Id != 0)
                    return BadRequest("Para atualizar um processo existente, utilizar o método PUT");

                processo.Advogado = null;
                processo.CreatedAt = DateTime.Now;
                processo.Honorarios = null;
                processo.ProcessoDetalhes = null;
                processo.Servicos = null;

                db.Processos.Add(processo);

                if (processo.Local == TipoJusticaEnum.Outro && string.IsNullOrEmpty(processo.Numero))
                    processo.Numero = processo.Id.ToString();

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message + ex.InnerException.Message);
            }

            processo.Advogado = db.Clientes.SingleOrDefault(x => x.Id == processo.AdvogadoId);
            return Ok(processo);
        }

        [HttpPost]
        [Route("detalhe")]
        public IActionResult PostProcessoDetalhe([FromBody] ProcessoDetalhe processoDetalhe)
        {
            try
            {
                var user = userManager.GetUserAsync(HttpContext.User).Result;
                processoDetalhe.UserId = user.Id;
                processoDetalhe.Data = DateTime.Now;

                var processo = db.Processos
                                .Include(x => x.ProcessoDetalhes)
                                .Single(x => x.Id == processoDetalhe.ProcessoId);

                processo.ProcessoDetalhes.Add(processoDetalhe);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult PutProcesso([FromBody] Processo newItem)
        {
            var item = db.Processos.Single(x => x.Id == newItem.Id);

            item.Numero = newItem.Numero;
            item.Autor = newItem.Autor;
            item.Reu = newItem.Reu;
            item.Local = newItem.Local;
            item.Parte = newItem.Parte;
            item.Vara = newItem.Vara;
            item.AdvogadoId = newItem.AdvogadoId;
            item.Indicacao = newItem.Indicacao;
            item.Perito = newItem.Perito;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProcesso(int id)
        {
            var item = db.Processos
                        .Include(x => x.Honorarios)
                        .Include(x => x.Servicos)
                        .Single(x => x.Id == id);

            if (item.Honorarios?.Count > 0 || item.Servicos?.Count > 0)
            {
                return BadRequest("O processo possúi Honorários/Serviços associados e não pode ser excluído");
            }
            else
            {
                db.Processos.Remove(item);
                db.SaveChanges();
                return Ok(item);
            }
        }

        [Route("local")]
        [HttpGet]
        public IActionResult GetLocal()
        {
            var result = Enum.GetValues(typeof(TipoJusticaEnum))
                            .Cast<TipoJusticaEnum>()
                            .Select(x => new { Key = (int)x, Value = EnumHelpers.GetEnumDescription(x), Mask = EnumHelpers.GetAttributeOfType<MaskAttribute>(x) });
            return Ok(result);
        }

        [Route("parte")]
        [HttpGet]
        public IActionResult GetParte()
        {
            var result = Enum.GetValues(typeof(ParteEnum))
                            .Cast<ParteEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [HttpGet("vara/{filter?}")]
        public IActionResult GetVara([FromQuery] string filter = "")
        {
            var result = db.Processos.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Vara.ContainsIgnoreNonSpacing(filter))
                                    .Select(x => x.Vara)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

            return Ok(result);
        }

        [HttpGet("indicacao/{filter?}")]
        public IActionResult GetIndicacao([FromQuery] string filter = "")
        {
            var result = db.Processos.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Indicacao.ContainsIgnoreNonSpacing(filter))
                                    .Select(x => x.Indicacao)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

            return Ok(result);
        }

        [HttpGet("perito/{filter?}")]
        public IActionResult GetPerito([FromQuery] string filter = "")
        {
            var result = db.Processos.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Perito.ContainsIgnoreNonSpacing(filter))
                                    .Select(x => x.Perito)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

            return Ok(result);
        }
    }
}
