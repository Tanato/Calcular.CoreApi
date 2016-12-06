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

        [HttpGet()]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Propostas
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || x.Numero.Contains(filter)
                                        || x.Contato.Contains(filter))
                                        .OrderBy(x => x.Id)
                            .Select(x => new Proposta
                            {
                                Contato = x.Contato,
                                Numero = x.Numero,
                                Id = x.Id,
                                Telefone = x.Telefone,
                                Celular = x.Celular,
                            })
                            .ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("numero")]
        public IActionResult GetByNumber([FromQuery] string filter)
        {
            var result = db.Propostas
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || x.Numero.Contains(filter))
                            .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Propostas
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostProposta([FromBody] Proposta proposta)
        {
            var user = userManager.GetUserAsync(HttpContext.User).Result;

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
            var contato = db.Propostas.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Contato.ContainsIgnoreNonSpacing(filter))
                                    .GroupBy(c => c.Contato)
                                    .Select(g => g.First())
                                    .ToList()
                                    .Select(x => new Cliente
                                    {
                                        Nome = x.Contato,
                                        Id = 0,
                                        Email = x.Email,
                                        Telefone = x.Telefone,
                                        Celular = x.Celular,
                                    })
                                    .OrderBy(x => x.Nome)
                                    .ToList();

            var cliente = db.Clientes.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Nome.ContainsIgnoreNonSpacing(filter))
                                    .Select(x => new Cliente
                                    {
                                        Nome = x.Nome,
                                        Id = x.Id,
                                        Email = x.Email,
                                        Telefone = x.Telefone,
                                        Celular = x.Celular,
                                    })
                                    .OrderBy(x => x.Nome)
                                    .ToList();

            contato.RemoveAll(x => cliente.Any(c => c.Nome == x.Nome
                                                || c.Email == x.Email
                                                || c.Celular == x.Celular
                                                || c.Telefone == x.Telefone));

            cliente.AddRange(contato);

            return Ok(cliente);
        }

        [Route("comochegou")]
        [HttpGet]
        public IActionResult GetComoChegou()
        {
            var result = Enum.GetValues(typeof(ComoChegouEnum))
                            .Cast<ComoChegouEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [Route("motivo")]
        [HttpGet]
        public IActionResult GetMotivo()
        {
            var result = Enum.GetValues(typeof(MotivoEnum))
                            .Cast<MotivoEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }
    }
}
