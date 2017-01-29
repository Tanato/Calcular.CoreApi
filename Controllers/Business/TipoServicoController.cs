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
    public class TipoServicoController : Controller
    {
        private readonly ApplicationDbContext db;

        public TipoServicoController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(db.TipoServicos.ToList());
        }

        [HttpGet("select")]
        public IActionResult GetSelect()
        {
            return Ok(db.TipoServicos.Select(x => new KeyValuePair<int, string>(x.Id, x.Nome)));
        }

        [HttpPost]
        public IActionResult Post([FromBody] TipoServico tipoServico)
        {
            if (string.IsNullOrEmpty(tipoServico.Nome))
                return BadRequest("Nome do tipo de serviço não pode ser nulo");
            else if (db.TipoServicos.Any(x => x.Nome == tipoServico.Nome))
                return BadRequest("Já existe um tipo de serviço com este nome.");
            
            db.TipoServicos.Add(tipoServico);
            db.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (db.Servicos.Any(x => x.TipoServicoId == id))
                return BadRequest("Existem tipos de serviço cadastrados com este tipo, tipo não pode ser excluído");

            var item = db.TipoServicos.Single(x => x.Id == id);

            db.TipoServicos.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }
    }
}
