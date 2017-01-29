using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
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

            var result = db.Atividades
                        .Include(x => x.Responsavel)
                        .Include(x => x.Servico).ThenInclude(x => x.Processo).ThenInclude(x => x.Advogado)
                        .Include(x => x.TipoAtividade)
                        .Include(x => x.AtividadeOrigem).ThenInclude(x => x.Responsavel)
                        .Where(x => x.Servico != null && x.Servico.Processo != null
                                    && (x.ResponsavelId == user.Id || (isRevisor && x.EtapaAtividade == EtapaAtividadeEnum.Revisao)) // Filtra por usuário ou revisor
                                    && (all || x.TipoExecucao == TipoExecucaoEnum.Pendente)) // Filtra atividades pendentes
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
        public IActionResult GetById(int id)
        {
            var result = db.Atividades
                            .Include(x => x.Responsavel)
                            .Include(x => x.Servico).ThenInclude(x => x.Processo).ThenInclude(x => x.Advogado)
                            .Include(x => x.TipoAtividade)
                            .Include(x => x.AtividadeOrigem).ThenInclude(x => x.Responsavel)
                            .Include(x => x.AtividadeOrigem).ThenInclude(x => x.TipoAtividade)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Atividade atividade)
        {
            db.Atividades.Add(atividade);
            db.SaveChanges();

            return Ok(atividade);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Atividade newItem)
        {
            var item = db.Atividades.Single(x => x.Id == newItem.Id);

            if (item.EtapaAtividade == EtapaAtividadeEnum.Revisao || newItem.EtapaAtividade == EtapaAtividadeEnum.Revisao)
                return BadRequest("Atividade de Revisão não pode ser alterada.");

            if (item.EtapaAtividade == EtapaAtividadeEnum.Refazer || newItem.EtapaAtividade == EtapaAtividadeEnum.Refazer)
                return BadRequest("Atividade para Refazer não pode ser alterada.");

            item.Entrega = newItem.Entrega;
            item.Tempo = newItem.Tempo;
            item.TipoImpressao = newItem.TipoImpressao;
            item.Observacao = newItem.Observacao;
            item.TipoAtividadeId = newItem.TipoAtividadeId;
            item.ResponsavelId = newItem.ResponsavelId;

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
        public IActionResult GetResponsavel()
        {
            var item = db.Users.ToList()
                            .Where(x => (userManager.IsInRoleAsync(x, "Calculista").Result)
                                     && !x.Inativo)
                            .Select(x => new KeyValuePair<string, string>(x.Id, x.Name));
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
    }
}
