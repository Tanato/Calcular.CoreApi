using Calcular.CoreApi.BusinessObjects;
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
    public class AtividadeResponsavelController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public AtividadeResponsavelController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string filter)
        {
            //var user = userManager.GetUserAsync(HttpContext.User).Result;
            //var isRevisor = await userManager.IsInRoleAsync(user, "Revisor");

            var result = db.Atividades
                        .Include(x => x.Responsavel)
                        .Include(x => x.Servico).ThenInclude(x => x.Processo).ThenInclude(x => x.Advogado)
                        .Include(x => x.TipoAtividade)
                        .Include(x => x.AtividadeOrigem).ThenInclude(x => x.Responsavel)
                        .Where(x => x.Servico != null && x.Servico.Processo != null
                                    && (x.ResponsavelId != null) // Filtra por usuário ou revisor
                                    && (x.TipoExecucao == TipoExecucaoEnum.Pendente)) // Filtra atividades pendentes
                        .GroupBy(x => x.Responsavel).ToList()
                        .OrderBy(g => g.Count())
                        .Select(g => new User
                        {
                            UserName = g.Key.UserName,
                            Id = g.Key.Id,
                            Name = g.Key.Name,
                            Atividades = g.OrderBy(x => x.Servico.Prazo).Where(x => x.ResponsavelId == g.Key.Id).Select(x =>
                                new Atividade
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
                                }).ToList(),
                        });
            return Ok(result);
        }
    }
}
