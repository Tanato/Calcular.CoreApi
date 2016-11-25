using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class TipoAtividadeController : Controller
    {
        private readonly ApplicationDbContext db;

        public TipoAtividadeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(db.TipoAtividades.ToList());
        }

        [HttpPost]
        public IActionResult Post([FromBody] TipoAtividade tipoAtividade)
        {
            if (string.IsNullOrEmpty(tipoAtividade.Nome))
            {
                return BadRequest("Nome do tipo de atividade não pode ser nulo");
            }
            db.TipoAtividades.Add(tipoAtividade);
            db.SaveChanges();
            return Ok();
        }

    }
}
