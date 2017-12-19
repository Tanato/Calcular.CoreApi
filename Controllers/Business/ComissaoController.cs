using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ComissaoController : Controller
    {
        private readonly ApplicationDbContext db;

        public ComissaoController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int filter)
        {
            return Ok(db.Comissoes.Include(x => x.TipoAtividade)
                                    .Where(x => x.Ativo && (filter == 0 || x.TipoAtividade.Id == filter))
                                    .OrderBy(x => x.TipoAtividade.Nome).OrderByDescending(a => a.Vigencia.Date)
                                    .ToList());
        }

        [HttpPost]
        public IActionResult Post([FromBody] Comissao comissao)
        {
            comissao.Ativo = true;
            comissao.Vigencia = comissao.Vigencia.Date.AddHours(12);
            db.Comissoes.Add(comissao);
            db.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Comissoes.Single(x => x.Id == id);

            item.Ativo = false;

            db.SaveChanges();
            return Ok(item);
        }
    }
}
