﻿using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class CobrancaController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public CobrancaController(UserManager<User> userManager, ApplicationDbContext db)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter, [FromQuery] bool all)
        {
            var pendente = filter.ContainsIgnoreNonSpacing("pendente");
            var pago = filter.ContainsIgnoreNonSpacing("pago");
            var atrasado = filter.ContainsIgnoreNonSpacing("atrasado");

            DateTime filterDate;
            bool isDate = DateTime.TryParse(filter, out filterDate);

            var query = db.Processos
                        .Include(x => x.Advogado)//.ThenInclude(x => x.Processos).ThenInclude(x => x.Cobrancas)
                        .Include(x => x.Honorarios)
                        .Include(x => x.Cobrancas).ThenInclude(x => x.Usuario)
                        .Where(x => x.Honorarios.Any()
                                    && (isDate
                                        || (string.IsNullOrEmpty(filter)
                                        || pago || pendente || atrasado
                                        || x.Numero.Contains(filter)
                                        || x.Advogado.Nome.Contains(filter)
                                        || x.Advogado.Empresa.Contains(filter))));


            // Se busca por status de honorário, concretiza a busca antes de filtrar.
            if (pago || pendente || atrasado)
            {
                query = query.ToList()
                             .Where(x => (pendente && x.StatusHonorario == "Pendente")
                                    || (pago && x.StatusHonorario == "Pago")
                                    || (atrasado && x.StatusHonorario == "Atrasado")).AsQueryable();
            }

            var ordered = query
                        .ToList()
                        .Where(x => (all || x.Total > 0)
                                    && (!isDate || (x.DataCobranca.HasValue && x.DataCobranca.Value.Date == filterDate.Date)));

            var result = ordered.GroupBy(x => new { x.Advogado, x.StatusHonorario }, (key, elements) => new CobrancaViewModel
                {
                    Advogado = key.Advogado,
                    AdvogadoId = key.Advogado.Id,
                    DataCobranca = elements.Any(p => p.Cobrancas.Any()) ? elements.Where(p => p.Cobrancas.Any()).Max(p => p.Cobrancas.Max(c => c.DataCobranca)) : (DateTime?)null,
                    TotalPendente = Convert.ToDecimal(elements.Sum(p => p.Total)),
                    TotalHonorarios = Convert.ToDecimal(elements.Sum(p => p.Honorario)),
                    TotalProcessosPendentes = elements.Count(),
                    //Processos = elements,
                    StatusHonorario = key.StatusHonorario,
                })
                .OrderBy(x => x.Advogado.Nome);

            return Ok(result);
        }

        [HttpGet]
        [Route("processo/numero")]
        public IActionResult GetByNumber([FromQuery] string filter)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.Cobrancas).ThenInclude(x => x.Usuario)
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || x.Numero.Contains(filter))
                            .ToList();

            return Ok(result);
        }

        [HttpGet("id")]
        public IActionResult GetById(int id, string status)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.Cobrancas).ThenInclude(x => x.Usuario)
                            .Where(x => x.Honorarios.Any()
                                    &&  x.Advogado.Id == id)
                            .ToList() 
                            .Where(x => x.StatusHonorario.ToUpper() == status.ToUpper());

            if (result.Any())
            {
                var cobranca = new CobrancaViewModel
                {
                    Advogado = result.First().Advogado,
                    AdvogadoId = result.First().Advogado.Id,
                    DataCobranca = result.Any(p => p.Cobrancas.Any()) ? result.Where(p => p.Cobrancas.Any()).Max(p => p.Cobrancas.Max(c => c.DataCobranca)) : (DateTime?)null,
                    TotalPendente = Convert.ToDecimal(result.Sum(p => p.Total)),
                    TotalHonorarios = Convert.ToDecimal(result.Sum(x => x.Honorario)),
                    TotalProcessosPendentes = result.Count(),
                    StatusHonorario = result.First().StatusHonorario,
                    Processos = result.Select(x => new ProcessoViewModel
                    {
                        Id = x.Id,
                        Numero = x.Numero,
                        Advogado = x.Advogado,
                        Autor = x.Autor,
                        Reu = x.Reu,
                        AdvogadoId = x.AdvogadoId,
                        Prazo = x.Prazo,
                        PrevisaoPagamento = x.PrevisaoPagamento,
                        DataCobranca = x.DataCobranca,
                        Vara = x.Vara,
                        NumeroAutores = x.NumeroAutores,
                        Local = x.Local,
                        Honorario = x.Honorario,
                        FaseProcesso = x.FaseProcesso,
                        StatusHonorario = x.StatusHonorario,
                        Parte = x.Parte,
                        Cobrancas = x.Cobrancas,
                        Total = x.Total,
                        UltimaCobranca = x.Cobrancas.OrderByDescending(c => c.DataCobranca).FirstOrDefault(),
                    }),
                };
                return Ok(cobranca);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult PostCobranca([FromBody] CobrancaViewModel model)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;

            foreach (var item in model.Processos)
            {
                var processo = db.Processos
                           .Include(x => x.Honorarios)
                           .Single(x => x.Id == item.Id);

                var cobranca = new Cobranca();

                cobranca.ProcessoId = processo.Id;
                cobranca.UsuarioId = user.Id;
                cobranca.DataCobranca = item.UltimaCobranca.DataCobranca;
                cobranca.Observacao = item.UltimaCobranca.Observacao;
                cobranca.PrevisaoPagamento = item.UltimaCobranca.PrevisaoPagamento;
                cobranca.Contato = item.UltimaCobranca.Contato;
                cobranca.ValorPendente = Convert.ToDecimal(processo.Total);

                db.Cobrancas.Add(cobranca);
                db.SaveChanges();
            }

            return Ok(model);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCobranca(int id)
        {
            var item = db.Cobrancas
                        .Single(x => x.Id == id);

            db.Cobrancas.Remove(item);
            db.SaveChanges();
            return Ok(item);

        }

        [HttpGet("contato/{filter?}")]
        public IActionResult GetContato([FromQuery] string filter = "")
        {
            var result = db.Cobrancas.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Contato.ContainsIgnoreNonSpacing(filter))
                                    .Select(x => x.Contato)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

            return Ok(result);
        }
    }
}
