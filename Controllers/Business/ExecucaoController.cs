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
        public async Task<IActionResult> ExecuteAtividade([FromBody] AtividadeViewModel model)
        {
            try
            {
                var user = userManager.GetUserAsync(HttpContext.User).Result;
                var isRevisor = await userManager.IsInRoleAsync(user, "Revisor");
                var isExterno = await userManager.IsInRoleAsync(user, "Colaborador Externo");
                var isCalculista = await userManager.IsInRoleAsync(user, "Calculista");

                var atividade = db.Atividades.Include(x => x.TipoAtividade).Single(x => x.Id == model.Id);

                if (atividade.TipoExecucao != TipoExecucaoEnum.Pendente)
                    return BadRequest("Só é possível executar atividades no estado Pendente");

                // Adiciona o usuário atual como responsável pela execução da revisão
                if (isRevisor && atividade.EtapaAtividade == EtapaAtividadeEnum.Revisao)
                    atividade.ResponsavelId = user.Id;

                if (atividade.ResponsavelId != user.Id)
                    return BadRequest("Usuário não autorizado à executar esta atividade");

                atividade.Entrega = model.Entrega;

                if (!string.IsNullOrEmpty(model.Tempo) && model.Tempo.Split(':').Length != 2)
                    return BadRequest("Tempo de execução inválido");

                if (isCalculista)
                {
                    atividade.Tempo = string.IsNullOrEmpty(model.Tempo)
                                    ? (TimeSpan?)null
                                    : new TimeSpan(int.Parse(model.Tempo.Split(':')[0]),    // hours
                                              int.Parse(model.Tempo.Split(':')[1]),    // minutes
                                              0);                               // seconds
                }

                if (atividade.TipoExecucao != TipoExecucaoEnum.Finalizado)
                    atividade.Observacao = model.Observacao;

                atividade.ObservacaoRevisor = model.ObservacaoRevisor;
                atividade.ObservacaoComissao = model.ObservacaoComissao;
                atividade.TipoExecucao = model.TipoExecucao;

                switch (model.TipoExecucao)
                {
                    case TipoExecucaoEnum.Pendente:
                        return BadRequest("Favor selecionar o status de execução diferente de Pendente.");

                    case TipoExecucaoEnum.Finalizado:
                        atividade.TipoAtividadeId = model.TipoAtividadeId;
                        atividade.TipoImpressao = model.TipoImpressao;
                        break;

                    case TipoExecucaoEnum.Revisar:
                        if (model.EtapaAtividade == EtapaAtividadeEnum.Revisao)
                            return BadRequest("Revisar atividade não permitido pois atividade já é de revisão");

                        atividade.TipoAtividadeId = model.TipoAtividadeId;
                        atividade.TipoImpressao = model.TipoImpressao;

                        db.Atividades.Add(new Atividade
                        {
                            ServicoId = atividade.ServicoId,
                            AtividadeOrigemId = atividade.Id,
                            TipoImpressao = atividade.TipoImpressao,
                            TipoAtividadeId = model.TipoAtividadeId,
                            EtapaAtividade = EtapaAtividadeEnum.Revisao,
                        });
                        break;

                    case TipoExecucaoEnum.Refazer:
                        var atividadeOrigem = db.Atividades.Single(x => x.Id == atividade.AtividadeOrigemId);

                        if (!isRevisor)
                            return Unauthorized();
                        else if (atividadeOrigem == null)
                            return BadRequest("Atividade de revisão não relacionada à nenhuma atividade, impossível refazer");
                        else if (model.EtapaAtividade != EtapaAtividadeEnum.Revisao)
                            return BadRequest("Refazer atividade permitido apenas na etapa de Revisão");

                        db.Atividades.Add(new Atividade
                        {
                            ServicoId = atividadeOrigem.ServicoId,
                            AtividadeOrigemId = atividade.Id, // Etapa de revisão como atividade que originou a refazer
                            ResponsavelId = atividadeOrigem.ResponsavelId,
                            TipoImpressao = atividadeOrigem.TipoImpressao,
                            TipoAtividadeId = atividadeOrigem.TipoAtividadeId,
                            EtapaAtividade = EtapaAtividadeEnum.Refazer,
                        });
                        break;

                    case TipoExecucaoEnum.RefazerCancelado:
                    case TipoExecucaoEnum.RevisarCancelado:
                    case TipoExecucaoEnum.Cancelado:
                        return BadRequest("Atividade cancelada, impossível executar.");
                    default:
                        break;
                }


                var servico = db.Servicos.Single(x => x.Id == atividade.ServicoId);

                switch (atividade.TipoImpressao)
                {
                    case TipoImpressaoEnum.Excel:
                        if (!servico.TipoImpressao.HasValue)
                            servico.TipoImpressao = atividade.TipoImpressao;
                        else if (servico.TipoImpressao == TipoImpressaoEnum.Word)
                            servico.TipoImpressao = TipoImpressaoEnum.WordExcel;
                        break;
                    case TipoImpressaoEnum.Word:
                        if (!servico.TipoImpressao.HasValue)
                            servico.TipoImpressao = atividade.TipoImpressao;
                        else if (servico.TipoImpressao == TipoImpressaoEnum.Excel)
                            servico.TipoImpressao = TipoImpressaoEnum.WordExcel;
                        break;
                    case TipoImpressaoEnum.WordExcel:
                        if (!servico.TipoImpressao.HasValue)
                            servico.TipoImpressao = atividade.TipoImpressao;
                        break;
                    default:
                        break;
                }

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
