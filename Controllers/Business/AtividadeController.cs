using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Models.ViewModels;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class AtividadeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public AtividadeController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Atividades
                            .Include(x => x.Responsavel)
                            .Include(x => x.Servico).ThenInclude(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.TipoAtividade)
                            .Include(x => x.AtividadeOrigem)
                            .Select(x => new Atividade
                            {
                                Id = x.Id,
                                Entrega = x.Entrega,
                                Tempo = x.Tempo,
                                TipoExecucao = x.TipoExecucao,
                                ServicoId = x.ServicoId,
                                Servico = new Servico
                                {
                                    Id = x.Servico.Id,
                                    Prazo = x.Servico.Prazo,
                                    Status = x.Servico.Status,
                                    Processo = new Processo
                                    {
                                        Id = x.Servico.Processo.Id,
                                        Numero = x.Servico.Processo.Numero,
                                        Autor = x.Servico.Processo.Autor,
                                        Reu = x.Servico.Processo.Reu,
                                    }
                                },
                                EtapaAtividade = x.EtapaAtividade,
                                AtividadeOrigemId = x.AtividadeOrigemId,
                                TipoAtividadeId = x.TipoAtividadeId,
                                TipoAtividade = new TipoAtividade
                                {
                                    Id = x.TipoAtividadeId,
                                    Nome = x.TipoAtividade.Nome
                                }

                            })
                            .ToList();
            return Ok(result);
        }

        [HttpGet("currentuser")]
        public async Task<IActionResult> GetAtividadesByUser([FromQuery] string filter, [FromQuery] bool all = false)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            var isRevisor = await userManager.IsInRoleAsync(user, "Revisor");

            var isOriginal = !string.IsNullOrEmpty(filter) ? filter.Contains("original") : false;
            var isRefazer = !string.IsNullOrEmpty(filter) ? filter.Contains("refazer") : false;
            var isRevisar = !string.IsNullOrEmpty(filter) ? filter.Contains("revisar") : false;

            var result = db.Atividades
                        .Include(x => x.Responsavel)
                        .Include(x => x.Servico).ThenInclude(x => x.Processo).ThenInclude(x => x.Advogado)
                        .Include(x => x.TipoAtividade)
                        .Include(x => x.AtividadeOrigem).ThenInclude(x => x.Responsavel)
                        .Where(x => x.Servico != null && x.Servico.Processo != null
                                    && (x.ResponsavelId == user.Id || (isRevisor && x.EtapaAtividade == EtapaAtividadeEnum.Revisao)) // Filtra por usuário ou revisor
                                    && (!isOriginal || x.EtapaAtividade == EtapaAtividadeEnum.Original)
                                    && (!isRefazer || x.EtapaAtividade == EtapaAtividadeEnum.Refazer)
                                    && (!isRevisar || x.EtapaAtividade == EtapaAtividadeEnum.Revisao)
                                    && (all || x.TipoExecucao == TipoExecucaoEnum.Pendente)) // Filtra atividades pendentes
                        .OrderBy(x => x.Servico.Prazo)
                        .ToList()
                        .Select(x => new Atividade
                        {
                            Id = x.Id,
                            Entrega = x.Entrega,
                            Tempo = x.Tempo,
                            TipoExecucao = x.TipoExecucao,
                            ServicoId = x.ServicoId,
                            Servico = new Servico
                            {
                                Id = x.Servico.Id,
                                Prazo = x.Servico.Prazo,
                                Status = x.Servico.Status,
                                Processo = new Processo
                                {
                                    Id = x.Servico.Processo.Id,
                                    Numero = x.Servico.Processo.Numero,
                                    Autor = x.Servico.Processo.Autor,
                                    Reu = x.Servico.Processo.Reu,
                                }
                            },
                            EtapaAtividade = x.EtapaAtividade,
                            AtividadeOrigemId = x.AtividadeOrigemId,
                            AtividadeOrigem = x.AtividadeOrigem == null ? null : new Atividade
                            {
                                Id = x.AtividadeOrigem.Id,
                                Responsavel = new User
                                {
                                    Id = x.AtividadeOrigem?.Responsavel?.Id,
                                    Name = x.AtividadeOrigem?.Responsavel?.Name,
                                }
                            },
                            TipoAtividadeId = x.TipoAtividadeId,
                            TipoAtividade = new TipoAtividade
                            {
                                Id = x.TipoAtividadeId,
                                Nome = x.TipoAtividade.Nome
                            }
                        });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            var isRevisor = await userManager.IsInRoleAsync(user, "Revisor");
            var isAdmOrGer = await userManager.IsInRoleAsync(user, "Administrativo") || await userManager.IsInRoleAsync(user, "Gerencial");

            var r = db.Atividades
                            .Include(x => x.Responsavel)
                            .Include(x => x.Servico).ThenInclude(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.TipoAtividade)
                            .Include(x => x.AtividadeOrigem).ThenInclude(x => x.Responsavel)
                            .Include(x => x.AtividadeOrigem).ThenInclude(x => x.TipoAtividade)
                            .SingleOrDefault(x => x.Id == id);

            var result = new AtividadeViewModel
            {
                Id = r.Id,
                AtividadeOrigem = r.AtividadeOrigem,
                AtividadeOrigemId = r.AtividadeOrigemId,
                Entrega = r.Entrega,
                EtapaAtividade = r.EtapaAtividade,
                Observacao = r.Observacao,
                ObservacaoRevisor = isRevisor ? r.ObservacaoRevisor : string.Empty,
                ObservacaoComissao = r.ObservacaoComissao,
                Responsavel = r.Responsavel,
                ResponsavelId = r.ResponsavelId,
                Servico = r.Servico,
                ServicoId = r.ServicoId,
                Tempo = GetHourFromTimespan(r.Tempo),
                TipoAtividade = r.TipoAtividade,
                TipoAtividadeId = r.TipoAtividadeId,
                TipoExecucao = r.TipoExecucao,
                TipoImpressao = r.TipoImpressao,
            };

            if (isAdmOrGer || user.Id == result.ResponsavelId)
                return Ok(result);
            else
                return BadRequest("Usuário não autorizado à visualizar atividade");

        }

        private static string GetHourFromTimespan(TimeSpan? tempo)
        {
            if (tempo.HasValue)
            {
                var dias = !string.IsNullOrEmpty(tempo.Value.ToString(@"dd")) ? Convert.ToInt16(tempo.Value.ToString(@"dd")) : 0;
                var horas = !string.IsNullOrEmpty(tempo.Value.ToString(@"hh")) ? Convert.ToInt16(tempo.Value.ToString(@"hh")) : 0;
                var minutos = tempo.Value.ToString(@"mm");

                var h = dias * 24 + horas;
                return $"{h.ToString("D2")}:{minutos}";

            }
            else
                return string.Empty;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Atividade model)
        {
            var atividade = new Atividade
            {
                TipoAtividadeId = model.TipoAtividadeId,
                ResponsavelId = model.ResponsavelId,
                AtividadeOrigemId = model.AtividadeOrigemId,
                EtapaAtividade = model.EtapaAtividade,
                Observacao = model.Observacao,
                ObservacaoComissao = model.ObservacaoComissao,
                ObservacaoRevisor = model.ObservacaoRevisor,
                Entrega = model.Entrega,
                ServicoId = model.ServicoId,
                Tempo = model.Tempo,
                TipoImpressao = model.TipoImpressao,
                Valor = model.Valor,
                TipoExecucao = model.TipoExecucao,
            };

            db.Atividades.Add(atividade);
            db.SaveChanges();

            return Ok(model);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Atividade model)
        {
            var item = db.Atividades.Single(x => x.Id == model.Id);

            if (item.EtapaAtividade == EtapaAtividadeEnum.Revisao || model.EtapaAtividade == EtapaAtividadeEnum.Revisao)
                return BadRequest("Atividade de Revisão não pode ser alterada.");

            if (item.EtapaAtividade == EtapaAtividadeEnum.Refazer || model.EtapaAtividade == EtapaAtividadeEnum.Refazer)
                return BadRequest("Atividade para Refazer não pode ser alterada.");

            item.Entrega = model.Entrega;
            item.Tempo = model.Tempo;
            item.TipoImpressao = model.TipoImpressao;
            item.Observacao = model.Observacao;
            item.TipoAtividadeId = model.TipoAtividadeId;
            item.ResponsavelId = model.ResponsavelId;
            item.Valor = model.Valor;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Atividades.Single(x => x.Id == id);

            if (item.TipoExecucao != TipoExecucaoEnum.Pendente)
                return BadRequest("Atividade não pode ser excluída");

            db.Atividades.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }

        [HttpGet("responsavel")]
        public async Task<IActionResult> GetResponsavel()
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            var isCalculista = await userManager.IsInRoleAsync(user, "Calculista");

            var item = db.Users
                            .ToList()
                            .Where(x => (userManager.IsInRoleAsync(x, "Calculista").Result
                                        || (userManager.IsInRoleAsync(x, "Colaborador Externo").Result && !isCalculista))
                                     && !x.Inativo)
                            .Select(x => new { Id = x.Id, Nome = x.Name, Login = x.Logins, IsExterno = userManager.IsInRoleAsync(x, "Colaborador Externo").Result });
            return Ok(item);
        }

        [HttpGet("tipoatividade")]
        public IActionResult GetTipoAtividade()
        {
            return Ok(db.TipoAtividades.ToList());
        }

        [Route("tipoimpressao")]
        [HttpGet]
        public IActionResult GetTipoImpressao()
        {
            var result = Enum.GetValues(typeof(TipoImpressaoEnum))
                            .Cast<TipoImpressaoEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [Route("tipoexecucao")]
        [HttpGet]
        public IActionResult GetTipoExecucao()
        {
            var result = Enum.GetValues(typeof(TipoExecucaoEnum))
                            .Cast<TipoExecucaoEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [HttpPost("observacao")]
        public async Task<IActionResult> EditObservacao([FromBody] AtividadeViewModel model)
        {
            try
            {
                var user = await userManager.GetUserAsync(HttpContext.User);

                var atividade = db.Atividades.Include(x => x.TipoAtividade).Single(x => x.Id == model.Id);

                if (atividade.ResponsavelId != user.Id)
                    return BadRequest("Usuário não autorizado à editar observação da atividade");

                atividade.ObservacaoComissao = model.ObservacaoComissao;

                db.SaveChanges();
                return Ok(atividade);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }
    }
}
