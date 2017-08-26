using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Calcular.CoreApi.Shared;
using System;
using System.Globalization;
using Calcular.CoreApi.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public ReportController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        /// <summary>
        /// Relatório de Custo/Processo agrupado por clientes.
        /// </summary>
        /// <returns></returns>
        [HttpGet("custoprocesso")]
        [Authorize(Roles = "Gerencial")]
        public async Task<IActionResult> GetCustoProcesso()
        {
            var clientes = await db.Clientes
                                .Include(x => x.Processos)
                                    .ThenInclude(x => x.Honorarios)
                                .Include(x => x.Processos)
                                    .ThenInclude(x => x.Servicos).ThenInclude(x => x.Atividades).ThenInclude(x => x.ComissaoAtividade)
                                .Where(x => x.Processos.Any())
                                .ToListAsync();

            var result = clientes.GroupBy(x => new { Cliente = x.Nome, Empresa = x.Empresa })
                                .Select(x =>
                                {
                                    var quantidadeProcessos = x.Sum(c => c.Processos.Count());
                                    var totalHonorario = x.Sum(c => c.Processos.Sum(p => p.Honorario));
                                    var totalCusto = x.Sum(c => c.Processos.Sum(p => p.Servicos.Sum(s => s.Atividades.Sum(a => a.ComissaoAtividade != null ? a.ComissaoAtividade.ValorFinal : 0))));

                                    return new
                                    {
                                        Cliente = x.Key.Cliente,
                                        Empresa = x.Key.Empresa,
                                        QuantidadeProcessos = quantidadeProcessos,
                                        TotalHonorario = totalHonorario,
                                        TotalCusto = totalCusto,
                                        Diferenca = totalHonorario - totalCusto,
                                        MediaHonorario = totalHonorario / quantidadeProcessos,
                                        MediaCusto = totalCusto / quantidadeProcessos,
                                    };
                                })
                                .OrderBy(x => x.Empresa).ThenBy(x => x.Cliente);

            return Ok(result);
        }

        /// <summary>
        /// Relatório de Produtividade por Colaborador
        /// </summary>
        /// <returns></returns>
        [HttpGet("produtividadecolaborador")]
        [Authorize(Roles = "Gerencial")]
        public async Task<IActionResult> GetProdutividadePorColaborador(DateTime? dataFim = null, int quantidadeMeses = 4)
        {
            var dataInicio = dataFim.HasValue ? dataFim.Value.AddMonths(-quantidadeMeses) : DateTime.Now.AddMonths(-quantidadeMeses);

            var data = await db.Atividades
                            .Include(x => x.Responsavel)
                            .Where(x => !string.IsNullOrEmpty(x.ResponsavelId)
                                        && x.Entrega.HasValue
                                        && (x.EtapaAtividade == EtapaAtividadeEnum.Original)
                                        && x.TipoExecucao == TipoExecucaoEnum.Finalizado || x.TipoExecucao == TipoExecucaoEnum.Revisar)
                            .ToListAsync();

            var revisores = data.Select(x => x.Responsavel.Name).Distinct();

            var group = data.GroupBy(x => new { Mes = new DateTime(x.Entrega.Value.Year, x.Entrega.Value.Month, 1), Responsavel = x.Responsavel })
                            .Select(x => new
                            {
                                Responsavel = x.Key.Responsavel.Name,
                                Mes = x.Key.Mes,
                                Atividades = x.Count(),
                            }).ToList();

            var result = new
            {
                Meses = Enumerable.Range(0, quantidadeMeses).Select(i => dataInicio.AddMonths(i).ToString("MMMM/yy", new CultureInfo("pt-PT"))),
                Report = revisores.Select(z => new
                {
                    Name = z,
                    Data = Enumerable.Range(0, quantidadeMeses).Select(i =>
                    {
                        var mes = dataInicio.AddMonths(i);
                        return group.Where(g => g.Responsavel == z && g.Mes == new DateTime(mes.Year, mes.Month, 1)).Select(g => g.Atividades);
                    }),
                }),
            };

            return Ok(result);
        }

        /// <summary>
        /// Relatório de Honorários por mês
        /// </summary>
        /// <returns></returns>
        [HttpGet("honorarios")]
        [Authorize(Roles = "Gerencial")]
        public IActionResult GetHonorarios(DateTime? dataFim = null, int quantidadeMeses = 4)
        {
            var dataInicio = dataFim.HasValue ? dataFim.Value.AddMonths(-quantidadeMeses + 1) : DateTime.Now.AddMonths(-quantidadeMeses + 1);
            dataInicio = new DateTime(dataInicio.Year, dataInicio.Month, 1);

            var meses = Enumerable.Range(0, quantidadeMeses).Select(i => dataInicio.AddMonths(i));

            var data = db.Honorarios.Include(x => x.Processo);

            var groupedHonorarios = data
                            .Where(x => x.Data >= dataInicio)
                            .GroupBy(x => new
                            {
                                Mes = new DateTime(x.Data.Year, x.Data.Month, 1),
                                Registro = x.Registro,
                                Oficial = x.Processo.Parte == ParteEnum.Oficial,
                            })
                            .Select(x => new
                            {
                                Mes = x.Key.Mes,
                                Oficial = x.Key.Oficial,
                                Registro = x.Key.Registro,
                                Valor = x.Sum(y => y.Valor),
                                Quantidade = x.Count(),

                            })
                            .OrderBy(x => x.Mes);

            var groupedTotal = data
                            .Where(x => x.Data >= dataInicio)
                            .GroupBy(x => new
                            {
                                Mes = new DateTime(x.Data.Year, x.Data.Month, 1),
                                Registro = x.Registro,
                            });

            var pendentes = data.Where(x => x.Processo.Total.HasValue);
            var groupedPendentes = pendentes.GroupBy(x => x.Processo.Parte == ParteEnum.Oficial)
                                            .Select(x => new
                                            {
                                                Oficial = x.Key,
                                                Total = x.Sum(y => y.Processo.Total),
                                                Quantidade = x.Count()
                                            });

            var result = new
            {
                Meses = meses.Select(m => m.ToString("MMMM/yy", new CultureInfo("pt-PT"))),
                FaturadosAssistencia = meses.Select(x =>
                                                    {
                                                        var h = groupedHonorarios.SingleOrDefault(g => g.Registro == RegistroEnum.Honorario && !g.Oficial && g.Mes == x);
                                                        return new { Valor = h != null ? h.Valor : 0, Quantidade = h != null ? h.Quantidade : 0 };
                                                    }),

                RecebidosAssistencia = meses.Select(x =>
                                                    {
                                                        var h = groupedHonorarios.SingleOrDefault(g => g.Registro == RegistroEnum.Pagamento && !g.Oficial && g.Mes == x);
                                                        return new { Valor = h != null ? h.Valor : 0, Quantidade = h != null ? h.Quantidade : 0 };
                                                    }),

                RecebidosOficial = meses.Select(x =>
                                                {
                                                    var h = groupedHonorarios.SingleOrDefault(g => g.Registro == RegistroEnum.Pagamento && g.Oficial && g.Mes == x);
                                                    return new { Valor = h != null ? h.Valor : 0, Quantidade = h != null ? h.Quantidade : 0 };
                                                }),

                TotalHonorarios = meses.Select(x =>
                                                {
                                                    var h = groupedHonorarios.Where(g => g.Registro == RegistroEnum.Pagamento && g.Mes == x);
                                                    return new { Valor = h != null ? h.Sum(y => y.Valor) : 0, Quantidade = h != null ? h.Sum(y => y.Quantidade) : 0 };
                                                }),

                PendenteAssistenciaValor = groupedPendentes.Single(x => !x.Oficial).Total,
                PendenteAssistenciaQuantidade = groupedPendentes.Single(x => !x.Oficial).Quantidade,
                PendenteOficialValor = groupedPendentes.Single(x => x.Oficial).Total,
                PendenteOficialQuantidade = groupedPendentes.Single(x => x.Oficial).Quantidade,
                TotalPendenteValor = groupedPendentes.Sum(x => x.Total),
                TotalPendenteQuantidade = groupedPendentes.Sum(x => x.Quantidade),
            };

            return Ok(result);
        }

        /// <summary>
        /// Relatório de Honorários por mês
        /// </summary>
        /// <returns></returns>
        [HttpGet("tipoprocesso")]
        [Authorize(Roles = "Gerencial")]
        public async Task<IActionResult> GetTipoProcessos(DateTime? dataFim = null, int quantidadeMeses = 12)
        {
            var dataInicio = dataFim.HasValue ? dataFim.Value.AddMonths(-quantidadeMeses + 1) : DateTime.Now.AddMonths(-quantidadeMeses + 1);
            dataInicio = new DateTime(dataInicio.Year, dataInicio.Month, 1);

            var meses = Enumerable.Range(0, quantidadeMeses).Select(i => dataInicio.AddMonths(i));

            var data = await db.Servicos
                               .Include(x => x.Processo).ThenInclude(x => x.Servicos)
                               .Where(x => x.Entrada >= dataInicio).ToListAsync();

            var grouped = data.GroupBy(x => new
            {
                Mes = new DateTime(x.Entrada.Year, x.Entrada.Month, 1),
                Oficial = x.Processo.Parte == ParteEnum.Oficial,
            })
                                .Select(x => new
                                {
                                    Mes = x.Key.Mes,
                                    Oficial = x.Key.Oficial,
                                    Quantidade = x.Count(),
                                })
                                .OrderBy(x => x.Mes);

            var novos = data.Where(x => x.Processo.Servicos.Count == 1)
                            .GroupBy(x => new DateTime(x.Entrada.Year, x.Entrada.Month, 1))
                            .Select(x => new
                            {
                                Mes = x.Key,
                                Quantidade = x.Count(),
                            })
                            .OrderBy(x => x.Mes);

            var result = new
            {
                Meses = meses.Select(m => m.ToString("MMM/yy", new CultureInfo("pt-PT"))),
                QuantidadeOficial = meses.Select(x => grouped.SingleOrDefault(g => g.Oficial && g.Mes == x)?.Quantidade ?? 0),
                QuantidadeAssistencia = meses.Select(x => grouped.SingleOrDefault(g => !g.Oficial && g.Mes == x)?.Quantidade ?? 0),
                QuantidadeNovos = meses.Select(x => novos.SingleOrDefault(g => g.Mes == x)?.Quantidade ?? 0),
            };

            return Ok(result);
        }

        /// <summary>
        /// Relatório de Honorários por mês
        /// </summary>
        /// <returns></returns>
        [HttpGet("tempoprodutividade")]
        [Authorize(Roles = "Gerencial")]
        public async Task<IActionResult> GetTempoProdutividade(DateTime? mes, bool? calculo)
        {
            var mesFiltro = mes.HasValue ? mes.Value : DateTime.Now;
            mesFiltro = new DateTime(mesFiltro.Year, mesFiltro.Month, 1);

            var calculistas = db.Users.Where(x => (userManager.IsInRoleAsync(x, "Calculista").Result)).ToList();

            var tipoAtividades = db.TipoAtividades.ToList();

            var source = db.Atividades
                               .Include(x => x.TipoAtividade)
                               .OrderBy(x => x.Responsavel.Name)
                               .Where(x => x.Tempo.HasValue
                                           && x.Entrega.HasValue
                                           && x.Entrega.Value.Year == mesFiltro.Year
                                           && x.EtapaAtividade == EtapaAtividadeEnum.Original
                                           && calculistas.Any(c => c.Id == x.ResponsavelId));

            var dataMesColaborador = await source.Where(x => x.Entrega.Value.Month == mesFiltro.Month)
                               .GroupBy(x => new
                               {
                                   Calculista = x.Responsavel,
                                   TipoAtividade = x.TipoAtividade,
                               })
                               .Select(x => new
                               {
                                   TipoAtividade = x.Key.TipoAtividade,
                                   Calculista = x.Key.Calculista.Name,
                                   Media = new TimeSpan((long)x.Average(y => y.Tempo.Value.Ticks)).TotalMilliseconds
                               })
                               .ToListAsync();

            var dataMesGeral = await source.Where(x => x.Entrega.Value.Month == mesFiltro.Month)
                               .GroupBy(x => new { TipoAtividade = x.TipoAtividade })
                               .Select(x => new
                               {
                                   TipoAtividade = x.Key.TipoAtividade,
                                   Media = new TimeSpan((long)x.Average(y => y.Tempo.Value.Ticks)).TotalMilliseconds
                               })
                               .ToListAsync();

            var dataAnoColaborador = await source
                               .GroupBy(x => new
                               {
                                   Calculista = x.Responsavel,
                                   TipoAtividade = x.TipoAtividade,
                               })
                               .Select(x => new
                               {
                                   TipoAtividade = x.Key.TipoAtividade,
                                   Calculista = x.Key.Calculista.Name,
                                   Media = new TimeSpan((long)x.Average(y => y.Tempo.Value.Ticks)).TotalMilliseconds
                               })
                               .ToListAsync();

            var dataAnoGeral = await source
                               .GroupBy(x => new { TipoAtividade = x.TipoAtividade })
                               .Select(x => new
                               {
                                   TipoAtividade = x.Key.TipoAtividade,
                                   Media = new TimeSpan((long)x.Average(y => y.Tempo.Value.Ticks)).TotalMilliseconds
                               })
                               .ToListAsync();


            #region Report Calculo
            var reportCalculo = Enumerable.Empty<object>().Select(x => new { Name = string.Empty, Type = string.Empty, Data = Enumerable.Empty<double>() }).ToList();
            reportCalculo.Add(new
            {
                Name = "Media Geral",
                Type = "areaspline",
                Data = tipoAtividades.Where(t => t.Nome.Contains("Cálculo"))
                                     .Select(t => dataMesGeral.SingleOrDefault(y => y.TipoAtividade.Id == t.Id)?.Media ?? 0),
            });
            reportCalculo.AddRange(dataMesColaborador.GroupBy(x => x.Calculista)
                                                     .Select(x => new
                                                     {
                                                         Name = x.Key,
                                                         Type = "spline",
                                                         Data = tipoAtividades.Where(t => t.Nome.Contains("Cálculo"))
                                                                              .Select(t => x.SingleOrDefault(y => y.TipoAtividade.Id == t.Id)?.Media ?? 0),
                                                     }));
            #endregion

            #region Report Levantamento
            var reportLevantamento = Enumerable.Empty<object>().Select(x => new { Name = string.Empty, Type = string.Empty, Data = Enumerable.Empty<double>() }).ToList();
            reportLevantamento.Add(new
            {
                Name = "Media Geral",
                Type = "areaspline",
                Data = tipoAtividades.Where(t => t.Nome.Contains("Levantamento"))
                                     .Select(t => dataMesGeral.SingleOrDefault(y => y.TipoAtividade.Id == t.Id)?.Media ?? 0),
            });
            reportLevantamento.AddRange(dataMesColaborador.GroupBy(x => x.Calculista)
                                                          .Select(x => new
                                                          {
                                                              Name = x.Key,
                                                              Type = "spline",
                                                              Data = tipoAtividades.Where(t => t.Nome.Contains("Levantamento"))
                                                                                   .Select(t => x.SingleOrDefault(y => y.TipoAtividade.Id == t.Id)?.Media ?? 0),
                                                          }));
            #endregion

            #region Report Demais
            var reportDemais = Enumerable.Empty<object>().Select(x => new { Name = string.Empty, Type = string.Empty, Data = Enumerable.Empty<double>() }).ToList();
            reportDemais.Add(new
            {
                Name = "Media Geral",
                Type = "areaspline",
                Data = tipoAtividades.Where(t => !t.Nome.Contains("Cálculo") && !t.Nome.Contains("Levantamento"))
                                     .Select(t => dataMesGeral.SingleOrDefault(y => y.TipoAtividade.Id == t.Id)?.Media ?? 0),
            });
            reportDemais.AddRange(dataMesColaborador.GroupBy(x => x.Calculista)
                                                    .Select(x => new
                                                    {
                                                        Name = x.Key,
                                                        Type = "spline",
                                                        Data = tipoAtividades.Where(t => !t.Nome.Contains("Cálculo") && !t.Nome.Contains("Levantamento"))
                                                                             .Select(t => x.SingleOrDefault(y => y.TipoAtividade.Id == t.Id)?.Media ?? 0),
                                                    }));
            #endregion

            #region Report Calculo Anual
            var reportCalculoAnual = Enumerable.Empty<object>().Select(x => new { Name = string.Empty, Type = string.Empty, Data = Enumerable.Empty<double>() }).ToList();
            reportCalculoAnual.Add(new
            {
                Name = "Media Geral",
                Type = "areaspline",
                Data = tipoAtividades.Where(t => t.Nome.Contains("Cálculo"))
                                     .Select(t => dataAnoGeral.SingleOrDefault(y => y.TipoAtividade.Id == t.Id)?.Media ?? 0),
            });
            reportCalculoAnual.AddRange(dataAnoColaborador.GroupBy(x => x.Calculista)
                                                          .Select(x => new
                                                          {
                                                              Name = x.Key,
                                                              Type = "spline",
                                                              Data = tipoAtividades.Where(t => t.Nome.Contains("Cálculo"))
                                                                                   .Select(t => x.SingleOrDefault(y => y.TipoAtividade.Id == t.Id)?.Media ?? 0),
                                                          }));
            #endregion

            var result = new
            {
                Calculos = tipoAtividades.Where(x => x.Nome.Contains("Cálculo")).Select(x => x.Nome),
                ReportCalculo = reportCalculo,

                Levantamentos = tipoAtividades.Where(x => x.Nome.Contains("Levantamento")).Select(x => x.Nome),
                ReportLevantamento = reportLevantamento,

                Demais = tipoAtividades.Where(x => !x.Nome.Contains("Cálculo") && !x.Nome.Contains("Levantamento")).Select(x => x.Nome),
                ReportDemais = reportDemais,

                CalculosAnual = tipoAtividades.Where(x => x.Nome.Contains("Cálculo")).Select(x => x.Nome),
                ReportCalculoAnual = reportCalculoAnual,
            };

            return Ok(result);
        }
    }
}
