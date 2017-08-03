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

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext db;

        public ReportController(ApplicationDbContext db)
        {
            this.db = db;
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
    }
}
