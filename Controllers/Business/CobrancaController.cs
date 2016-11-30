using Calcular.CoreApi.Common;
using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
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
    public class CobrancaController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public CobrancaController(UserManager<User> userManager, ApplicationDbContext db)
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
                            .Include(x => x.Cobrancas).ThenInclude(x => x.Usuario)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || x.Numero.Contains(filter)
                                             || x.Reu.Contains(filter)
                                             || x.Autor.Contains(filter)
                                             || x.Advogado.Nome.Contains(filter));

            return Ok(result.ToList());
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

        [HttpGet("processo/{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.Cobrancas).ThenInclude(x => x.Usuario)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostCobranca([FromBody] Cobranca cobranca)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;

            var processo = db.Processos
                           .Include(x => x.Honorarios)
                           .Single(x => x.Id == cobranca.ProcessoId);

            cobranca.UsuarioId = user.Id;
            cobranca.ValorPendente = Convert.ToDecimal(processo.Total);

            db.Cobrancas.Add(cobranca);
            db.SaveChanges();

            return Ok(cobranca);
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
