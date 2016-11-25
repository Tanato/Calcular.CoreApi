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

        [HttpGet("select")]
        public IActionResult GetSelect()
        {
            return Ok(db.TipoAtividades.Select(x => new KeyValuePair<int, string>(x.Id, x.Nome)));
        }

        [HttpPost]
        public IActionResult Post([FromBody] TipoAtividade tipoAtividade)
        {
            if (string.IsNullOrEmpty(tipoAtividade.Nome))
                return BadRequest("Nome do tipo de atividade não pode ser nulo");
            else if (db.TipoAtividades.Any(x => x.Nome == tipoAtividade.Nome))
                return BadRequest("Já existe uma atividade com este nome.");


            db.TipoAtividades.Add(tipoAtividade);
            db.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (db.Atividades.Any(x => x.TipoAtividadeId == id))
                return BadRequest("Existem atividades cadastradas com este tipo, tipo não pode ser excluído");

            var item = db.TipoAtividades.Single(x => x.Id == id);
            db.TipoAtividades.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }
    }
}
