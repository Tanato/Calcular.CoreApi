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

            if (item.TipoExecucao != TipoExecucaoEnum.Pendente)
                return BadRequest("Só é possível executar atividades no estado Pendente");

            // Adiciona o usuário atual como responsável pela execução da revisão
            if (isRevisor && item.EtapaAtividade == EtapaAtividadeEnum.Revisao)
                item.ResponsavelId = user.Id;

            if (item.ResponsavelId != user.Id)
                return BadRequest("Usuário não autorizado à executar esta atividade");

            item.Entrega = atividade.Entrega;
            item.Tempo = atividade.Tempo;
            item.Observacao = atividade.Observacao;
            item.TipoExecucao = atividade.TipoExecucao;

            switch (atividade.TipoExecucao)
            {
                case TipoExecucaoEnum.Pendente:
                    return BadRequest("Favor selecionar o status de execução diferente de Pendente.");

                case TipoExecucaoEnum.Finalizado:
                    item.TipoAtividadeId = atividade.TipoAtividadeId;
                    item.TipoImpressao = atividade.TipoImpressao;
                    break;

                case TipoExecucaoEnum.Revisar:
                    if (atividade.EtapaAtividade == EtapaAtividadeEnum.Revisao)
                        return BadRequest("Revisar atividade não permitido pois atividade já é de revisão");

                    item.TipoAtividadeId = atividade.TipoAtividadeId;
                    item.TipoImpressao = atividade.TipoImpressao;

                    db.Atividades.Add(new Atividade
                    {
                        ServicoId = item.ServicoId,
                        AtividadeOrigemId = item.Id,
                        TipoImpressao = item.TipoImpressao,
                        TipoAtividadeId = atividade.TipoAtividadeId,
                        EtapaAtividade = EtapaAtividadeEnum.Revisao,
                    });
                    break;

                case TipoExecucaoEnum.Refazer:
                    var atividadeOrigem = db.Atividades.Single(x => x.Id == item.AtividadeOrigemId);

                    if (!isRevisor)
                        return Unauthorized();
                    else if (atividadeOrigem == null)
                        return BadRequest("Atividade de revisão não relacionada à nenhuma atividade, impossível refazer");
                    else if (atividade.EtapaAtividade != EtapaAtividadeEnum.Revisao)
                        return BadRequest("Refazer atividade permitido apenas na etapa de Revisão");

                    db.Atividades.Add(new Atividade
                    {
                        ServicoId = atividadeOrigem.ServicoId,
                        AtividadeOrigemId = item.Id, // Etapa de revisão como atividade que originou a refazer
                        ResponsavelId = atividadeOrigem.ResponsavelId,
                        TipoImpressao = atividadeOrigem.TipoImpressao,
                        TipoAtividadeId = atividadeOrigem.TipoAtividadeId,
                        EtapaAtividade = EtapaAtividadeEnum.Refazer,
                    });
                    break;
                default:
                    break;
            }

            db.SaveChanges();
            return Ok(item);
        }
    }
}
