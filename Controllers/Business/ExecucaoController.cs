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
    public class ExecucaoController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public ExecucaoController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteAtividade([FromBody] Atividade atividade)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            var isRevisor = await userManager.IsInRoleAsync(user, "Revisor");

            var item = db.Atividades.Include(x => x.TipoAtividade).Single(x => x.Id == atividade.Id);

            // Adiciona o usuário atual como responsável pela execução da revisão
            if (isRevisor && item.TipoAtividade.Nome == "Revisão")
                item.ResponsavelId = user.Id;

            if (item.ResponsavelId == user.Id)
            {
                item.Entrega = atividade.Entrega;
                item.Tempo = atividade.Tempo;
                item.Observacao = atividade.Observacao;

                switch (atividade.TipoExecucao)
                {
                    case TipoExecucaoEnum.Finalizar:
                        item.TipoAtividadeId = atividade.TipoAtividadeId;
                        item.TipoImpressao = atividade.TipoImpressao;
                        break;

                    case TipoExecucaoEnum.Revisar:
                        item.TipoAtividadeId = atividade.TipoAtividadeId;
                        item.TipoImpressao = atividade.TipoImpressao;

                        db.Atividades.Add(new Atividade
                        {
                            ServicoId = item.ServicoId,
                            AtividadeOrigemId = item.Id,
                            TipoImpressao = item.TipoImpressao,
                            TipoAtividadeId = db.TipoAtividades.Single(x => x.Nome == "Revisão").Id,
                        });
                        break;

                    case TipoExecucaoEnum.Refazer:
                        if (item.AtividadeOrigemId != null && isRevisor)
                        {
                            var atividadeOrigem = db.Atividades.Single(x => x.Id == item.AtividadeOrigemId);
                            db.Atividades.Add(new Atividade
                            {
                                ServicoId = atividadeOrigem.ServicoId,
                                AtividadeOrigemId = item.Id,
                                ResponsavelId = atividadeOrigem.ResponsavelId,
                                TipoImpressao = atividadeOrigem.TipoImpressao,
                                TipoAtividadeId = atividadeOrigem.TipoAtividadeId,
                            });
                        }
                        else if (!isRevisor)
                        {
                            return Unauthorized();
                        }
                        else if (item.AtividadeOrigemId == null)
                        {
                            return BadRequest("Atividade de revisão não relacionada à nenhuma atividade, impossível refazer");
                        }
                        break;
                    default:
                        break;
                }

                db.SaveChanges();
                return Ok(item);
            }
            else
            {
                return BadRequest("Usuário não autorizado à executar esta atividade");
            }
        }
    }
}
