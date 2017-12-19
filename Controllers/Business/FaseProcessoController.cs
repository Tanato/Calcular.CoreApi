using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class FaseProcessoController : Controller
    {
        private readonly ApplicationDbContext db;

        public FaseProcessoController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(db.FaseProcessos.ToList());
        }

        [HttpGet("select")]
        public IActionResult GetSelect()
        {
            return Ok(db.FaseProcessos.Select(x => new KeyValuePair<int, string>(x.Id, x.Nome)));
        }

        [HttpPost]
        public IActionResult Post([FromBody] FaseProcesso tipoAtividade)
        {
            if (string.IsNullOrEmpty(tipoAtividade.Nome))
                return BadRequest("Nome da fase processual não pode ser nulo");
            else if (db.FaseProcessos.Any(x => x.Nome == tipoAtividade.Nome))
                return BadRequest("Já existe uma fase processual com este nome.");
            
            db.FaseProcessos.Add(tipoAtividade);
            db.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (db.Processos.Any(x => x.FaseProcessoId == id))
                return BadRequest("Existem processos cadastradas com esta fase processual, registro não pode ser excluído");

            var item = db.FaseProcessos.Single(x => x.Id == id);

            db.FaseProcessos.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }
    }
}
