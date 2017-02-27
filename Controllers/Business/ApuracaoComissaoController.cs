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
    public class ApuracaoComissaoController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public ApuracaoComissaoController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int mes, [FromQuery] int ano, [FromQuery] string funcionarioId = "")
        {
            try
            {
                var user = userManager.GetUserAsync(HttpContext.User).Result;
                var isAdminOrGerencial = await userManager.IsInRoleAsync(user, "Administrativo")
                                      || await userManager.IsInRoleAsync(user, "Gerencial");

                var mesFiltro = GetMesFiltro(mes, ano);

                var comissoes = db.Comissoes.ToList();

                var data = db.Atividades
                    .Include(x => x.ComissaoAtividade).ThenInclude(x => x.ComissaoFuncionarioMes).ThenInclude(x => x.Responsavel)
                    .Include(x => x.TipoAtividade)
                    .Include(x => x.Responsavel)
                    .Include(x => x.Servico).ThenInclude(x => x.Processo)
                    .Where(x => x.Servico.Saida.HasValue
                                && (x.TipoExecucao == TipoExecucaoEnum.Finalizado 
                                    || x.TipoExecucao == TipoExecucaoEnum.Revisar)
                                && x.EtapaAtividade == EtapaAtividadeEnum.Original
                                && x.Servico.Saida.Value.Month == mesFiltro.Month
                                && x.Servico.Saida.Value.Year == mesFiltro.Year
                                && ((isAdminOrGerencial && (string.IsNullOrEmpty(funcionarioId) || x.ResponsavelId == funcionarioId))
                                    || (string.IsNullOrEmpty(funcionarioId) || x.ResponsavelId == user.Id)))
                                    .ToList();
                var result = data
                    .Select(a => a.ComissaoAtividade == null ?
                                new ComissaoAtividade
                                {
                                    Id = 0,
                                    ComissaoFuncionarioMesId = 0,
                                    ValorBase = GetValorBase(comissoes, a),

                                    AtividadeId = a.Id,
                                    Atividade = new Atividade
                                    {
                                        Id = a.Id,
                                        Entrega = a.Entrega,
                                        TipoAtividade = a.TipoAtividade,
                                        Tempo = a.Tempo,
                                        ObservacaoComissao = a.ObservacaoComissao,
                                        ResponsavelId = a.ResponsavelId,
                                        Responsavel = new User
                                        {
                                            Id = a.Responsavel.Id,
                                            Name = a.Responsavel.Name,
                                        },
                                        ServicoId = a.ServicoId,
                                        Servico = new Servico
                                        {
                                            Id = a.Servico.Id,
                                            Saida = a.Servico.Saida,
                                            ProcessoId = a.Servico.ProcessoId,
                                            Processo = new Processo
                                            {
                                                Id = a.Servico.ProcessoId,
                                                Numero = a.Servico.Processo.Numero,
                                                NumeroAutores = a.Servico.Processo.NumeroAutores,
                                            },
                                        },
                                    }
                                } :
                                new ComissaoAtividade
                                {
                                    Id = a.ComissaoAtividade.Id,
                                    ValorBase = GetValorBase(comissoes, a),
                                    ValorAdicional = a.ComissaoAtividade.ValorAdicional,
                                    ValorFinal = a.ComissaoAtividade.ValorFinal,

                                    ComissaoFuncionarioMesId = a.ComissaoAtividade.ComissaoFuncionarioMesId,
                                    ComissaoFuncionarioMes = new ComissaoFuncionarioMes
                                    {
                                        Id = a.ComissaoAtividade.ComissaoFuncionarioMes.Id,
                                        Ano = a.ComissaoAtividade.ComissaoFuncionarioMes.Ano,
                                        Mes = a.ComissaoAtividade.ComissaoFuncionarioMes.Mes,
                                        ResponsavelId = a.ComissaoAtividade.ComissaoFuncionarioMes.ResponsavelId,
                                        Status = a.ComissaoAtividade.ComissaoFuncionarioMes.Status,
                                        Responsavel = new User
                                        {
                                            Id = a.ComissaoAtividade.ComissaoFuncionarioMes.Responsavel.Id,
                                            Name = a.ComissaoAtividade.ComissaoFuncionarioMes.Responsavel.Name,
                                        },
                                    },

                                    AtividadeId = a.Id,
                                    Atividade = new Atividade
                                    {
                                        Id = a.Id,
                                        Entrega = a.Entrega,
                                        TipoAtividade = a.TipoAtividade,
                                        Tempo = a.Tempo,
                                        ObservacaoComissao = a.ObservacaoComissao,
                                        ResponsavelId = a.ResponsavelId,
                                        Responsavel = new User
                                        {
                                            Id = a.Responsavel.Id,
                                            Name = a.Responsavel.Name,
                                        },
                                        ServicoId = a.ServicoId,
                                        Servico = new Servico
                                        {
                                            Id = a.Servico.Id,
                                            Saida = a.Servico.Saida,
                                            ProcessoId = a.Servico.ProcessoId,
                                            Processo = new Processo
                                            {
                                                Id = a.Servico.ProcessoId,
                                                Numero = a.Servico.Processo.Numero,
                                                NumeroAutores = a.Servico.Processo.NumeroAutores,
                                            },
                                        },
                                    }
                                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ComissaoFuncionarioMes model)
        {
            try
            {
                var user = userManager.GetUserAsync(HttpContext.User).Result;
                var isAdminOrGerencial = await userManager.IsInRoleAsync(user, "Administrativo")
                                      || await userManager.IsInRoleAsync(user, "Gerencial");

                if (!isAdminOrGerencial)
                    return Unauthorized();

                var comissoes = db.Comissoes.ToList();
                var atividades = db.Atividades
                                .Include(x => x.Servico)
                                .Where(x => model.ComissaoAtividades.Select(a => a.AtividadeId).Contains(x.Id)).ToList();

                // Valida se atividades possuem o mesmo mês de saída do serviço e se são do mesmo responsável e não estão duplicadas
                if (atividades.Any(x => !x.Servico.Saida.HasValue)
                    || atividades.Select(x => new DateTime(x.Servico.Saida.Value.Year, x.Servico.Saida.Value.Month, 1)).Distinct().Count() > 1
                    || atividades.Select(x => x.ResponsavelId).Distinct().Count() > 1
                    || model.ComissaoAtividades.Select(x => x.AtividadeId).GroupBy(x => x).Any(x => x.Skip(1).Any()))
                    return BadRequest("Atividades não relacionadas. [Servico.Saida, Atividade.Reponsável]");

                var existApuracao = db.ComissoesFuncionarioMes.Any(x => x.Mes == model.Mes && x.Ano == model.Ano && x.FuncionarioId == atividades.First().ResponsavelId);
                if (existApuracao)
                    return BadRequest("Já existe apuração para este funcionário neste mês, acionar método PUT");

                var dataMesFechamento = atividades.First().Servico.Saida.Value.AddMonths(1);

                var comissaoFuncionarioMes = new ComissaoFuncionarioMes
                {
                    Status = StatusApuracaoComissao.Aberto,
                    ResponsavelId = user.Id,
                    FuncionarioId = atividades.First().ResponsavelId,
                    Alteracao = DateTime.Now,
                    Ano = dataMesFechamento.Year,
                    Mes = dataMesFechamento.Month,
                    ComissaoAtividades = atividades.Select(a =>
                    {
                        var valBase = GetValorBase(comissoes, a);
                        var valAdicional = model.ComissaoAtividades.Single(c => c.AtividadeId == a.Id).ValorAdicional;
                        return new ComissaoAtividade
                        {
                            AtividadeId = a.Id,
                            ValorBase = valBase,
                            ValorAdicional = valAdicional,
                            ValorFinal = valAdicional + valBase,
                        };
                    }).ToList(),
                };

                db.ComissoesFuncionarioMes.Add(comissaoFuncionarioMes);

                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] ComissaoFuncionarioMes model)
        {
            try
            {
                var user = userManager.GetUserAsync(HttpContext.User).Result;
                var isAdminOrGerencial = await userManager.IsInRoleAsync(user, "Administrativo")
                                      || await userManager.IsInRoleAsync(user, "Gerencial");

                if (!isAdminOrGerencial)
                    return Unauthorized();

                var comissoes = db.Comissoes.ToList();
                var atividades = db.Atividades
                                .Include(x => x.Servico)
                                .Where(x => model.ComissaoAtividades.Select(a => a.AtividadeId).Contains(x.Id)).ToList();

                // Valida se atividades possuem o mesmo mês de saída do serviço e se são do mesmo responsável e não estão duplicadas
                if (atividades.Any(x => !x.Servico.Saida.HasValue)
                    || atividades.Select(x => new DateTime(x.Servico.Saida.Value.Year, x.Servico.Saida.Value.Month, 1)).Distinct().Count() > 1
                    || atividades.Select(x => x.ResponsavelId).Distinct().Count() > 1
                    || model.ComissaoAtividades.Select(x => x.AtividadeId).GroupBy(x => x).Any(x => x.Skip(1).Any()))
                    return BadRequest("Atividades não relacionadas. [Servico.Saida, Atividade.Reponsável]");

                var funcionarioMes = db.ComissoesFuncionarioMes.Single(x => x.Id == model.Id);

                if (funcionarioMes.Mes != model.Mes || funcionarioMes.Ano != model.Ano || funcionarioMes.FuncionarioId != atividades.First().ResponsavelId)
                    return BadRequest("Informações de Apuração de comissão por funcionário incorretas");

                funcionarioMes.ResponsavelId = user.Id;
                funcionarioMes.Alteracao = DateTime.Now;
                
                foreach (var modelItem in model.ComissaoAtividades)
                {
                    var dbComissaoAtividade = db.ComissaoAtividades
                                             .Include(x => x.Atividade)
                                             .SingleOrDefault(x => x.AtividadeId == modelItem.AtividadeId);

                    if (dbComissaoAtividade != null)
                    {
                        var valorBase = GetValorBase(comissoes, dbComissaoAtividade.Atividade);
                        dbComissaoAtividade.ValorBase = valorBase;
                        dbComissaoAtividade.ValorAdicional = modelItem.ValorAdicional;
                        dbComissaoAtividade.ValorFinal = modelItem.ValorAdicional + valorBase;
                    }
                    else
                    {
                        var a = db.Atividades.Single(x => x.Id == modelItem.AtividadeId);
                        var valBase = GetValorBase(comissoes, a);
                        db.ComissaoAtividades.Add(new ComissaoAtividade
                        {
                            ComissaoFuncionarioMesId = funcionarioMes.Id,
                            AtividadeId = a.Id,
                            ValorBase = valBase,
                            ValorAdicional = modelItem.ValorAdicional,
                            ValorFinal = modelItem.ValorAdicional + valBase,
                        });
                    }
                    db.SaveChanges();
                }

                return Ok(db.ComissoesFuncionarioMes.Single(x => x.Id == funcionarioMes.Id));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("fechaapuracao")]
        public async Task<IActionResult> FechaApuracao([FromQuery] int apuracaoId)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            var isAdminOrGerencial = await userManager.IsInRoleAsync(user, "Administrativo")
                                  || await userManager.IsInRoleAsync(user, "Gerencial");

            if (!isAdminOrGerencial)
                return Unauthorized();

            var comissaoFuncionarioMes = db.ComissoesFuncionarioMes.SingleOrDefault(x => x.Id == apuracaoId);

            if (comissaoFuncionarioMes == null)
                return BadRequest("Identificador inválido.");

            if (comissaoFuncionarioMes.Status == StatusApuracaoComissao.Fechado)
                return BadRequest("Comissão já fechada.");

            comissaoFuncionarioMes.ResponsavelId = user.Id;
            comissaoFuncionarioMes.Alteracao = DateTime.Now;
            comissaoFuncionarioMes.Status = StatusApuracaoComissao.Fechado;

            db.SaveChanges();
            return Ok();
        }

        private static DateTime GetMesFiltro(int mes, int ano)
        {
            return new DateTime(ano == 0 ? DateTime.Now.Year : ano, mes == 0 ? DateTime.Now.Month : mes, 1).AddMonths(-1);
        }

        private decimal GetValorBase(IEnumerable<Comissao> comissoes, Atividade atividade)
        {
            // Se atividade possui valor, considera como valor base.
            if (atividade.Valor.HasValue)
                return atividade.Valor.Value;
            else
            {
                var comissao = comissoes.Where(x => x.Ativo
                                                    && x.Vigencia <= DateTime.Now
                                                    && x.TipoAtividadeId == atividade.TipoAtividadeId
                                                    && atividade.Tempo <= (x.HoraMax != null ? x.HoraMax.Value : TimeSpan.MaxValue)
                                                    && atividade.Tempo >= (x.HoraMin != null ? x.HoraMin.Value : TimeSpan.MinValue))
                         .OrderByDescending(x => x.Vigencia)
                         .FirstOrDefault();

                return comissao != null ? comissao.Valor : 0;
            }
        }
    }
}
