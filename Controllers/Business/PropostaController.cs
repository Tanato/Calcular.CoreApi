using Calcular.CoreApi.Common;
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

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class PropostaController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public PropostaController(UserManager<User> userManager, ApplicationDbContext db)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet("processo")]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.Propostas).ThenInclude(x => x.Usuario)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || x.Numero.Contains(filter)
                                             || x.Reu.Contains(filter)
                                             || x.Autor.Contains(filter)
                                             || x.Advogado.Nome.Contains(filter))
                                             .OrderBy(x => x.Id)
                            .Select(x => new ProcessoViewModel
                            {
                                Advogado = new Cliente { Id = x.Advogado.Id, Nome = x.Advogado.Nome },
                                Numero = x.Numero,
                                Id = x.Id,
                                Autor = x.Autor,
                                Reu = x.Reu,
                            }).ToList(); ;

            return Ok(result.ToList());
        }

        [HttpGet]
        [Route("processo/numero")]
        public IActionResult GetByNumber([FromQuery] string filter)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.Propostas).ThenInclude(x => x.Usuario)
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || x.Numero.Contains(filter))
                            .ToList();

            return Ok(result);
        }

        [HttpGet("processo/{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.Propostas).ThenInclude(x => x.Usuario)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostProposta([FromBody] Proposta proposta)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;

            var processo = db.Processos
                           .Include(x => x.Honorarios)
                           .Single(x => x.Id == proposta.ProcessoId);

            proposta.UsuarioId = user.Id;

            db.Propostas.Add(proposta);
            db.SaveChanges();

            return Ok(proposta);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProposta(int id)
        {
            var item = db.Propostas
                        .Single(x => x.Id == id);

            db.Propostas.Remove(item);
            db.SaveChanges();
            return Ok(item);

        }

        [HttpGet("contato/{filter?}")]
        public IActionResult GetContato([FromQuery] string filter = "")
        {
            var result = db.Propostas.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Contato.ContainsIgnoreNonSpacing(filter))
                                    .Select(x => x.Contato)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

            return Ok(result);
        }
    }
}
