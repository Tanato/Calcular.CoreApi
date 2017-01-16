﻿using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Models.ViewModels;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class EventoController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public EventoController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;
            var days = GetNextDates();

            var result = db.Clientes.Where(x => x.Nascimento.HasValue && days.Any(d => d.DayOfYear == x.Nascimento.Value.DayOfYear))
                .Select(x => new EventoViewModel
                {
                    Id = x.Id,
                    Evento = "Aniversário",
                    Detalhe = x.Nome,
                    Data = x.Nascimento.Value,
                }).ToList();

            // Revisor e Calculista
            // Evento (aniversário, prazo do processo), Detalhe (nome do aniversariante ou numero do processo) e Data
            // Adicionar processos pra vencer o prazo de serviço relacionados ao usuário (mostrar o processo, independente a quantidade de atividades)
            if (userManager.IsInRoleAsync(user, "Revisor").Result || userManager.IsInRoleAsync(user, "Calculista").Result)
            {
                result.AddRange(
                    db.Servicos.Include(x => x.Atividades).Include(x => x.Processo)
                   .Where(x => x.Prazo.HasValue && days.Any(d => d.Date == x.Prazo.Value.Date)
                            && x.Atividades.Any(k => k.TipoExecucao != TipoExecucaoEnum.Finalizado && k.ResponsavelId == user.Id))
                    .Select(x => new EventoViewModel
                    {
                        Id = x.Id,
                        Evento = "Prazo do Processo",
                        Detalhe = x.Processo.Numero,
                        Data = x.Prazo.Value,
                        Status = "Pendente",
                    }));
            }

            // Gerencial e Administrativo
            // Evento, Detalhe, Responsável, Data, Observação**(digitavel)
            // Processo com todas as atividades finalizadas. (Finalizar processo)
            // Adicionar processos com prazos para vencer para todos os usuários e o nome dos usuários.
            if (userManager.IsInRoleAsync(user, "Gerencial").Result || userManager.IsInRoleAsync(user, "Administrativo").Result)
            {
                result.AddRange(
                   db.Servicos.Include(x => x.Atividades).Include(x => x.Processo)
                   .Where(x => x.Prazo.HasValue && days.Any(d => d.Date == x.Prazo.Value.Date))
                   .Select(x => new EventoViewModel
                   {
                       Id = x.Id,
                       Evento = "Prazo do Processo",
                       Detalhe = x.Processo.Numero,
                       Responsavel = string.Join(";", x.Atividades.Select(k => k.Responsavel != null ? k.Responsavel.Name : "")),
                       Data = x.Prazo.Value,
                       Status = x.Saida.HasValue ? "Entregue"
                                : x.Atividades.All(k => k.TipoExecucao == TipoExecucaoEnum.Finalizado) ? "Enviar"
                                : "Pendente",
                   }));
            }

            return Ok(result.OrderBy(x => x.Data));
        }

        /// <summary>
        /// Today and tomorrow, if friday show until monday.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DateTime> GetNextDates()
        {
            var today = DateTime.Now.DayOfWeek;
            return Enumerable.Range(0, today == DayOfWeek.Friday ? 4 : today == DayOfWeek.Saturday ? 3 : 2).Select(x => DateTime.Now.AddDays(x).Date);
        }
    }
}
