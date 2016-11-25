using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class AtividadeController : Controller
    {
        private readonly ApplicationDbContext db;

        public AtividadeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Atividades
                            .Include(x => x.Responsavel)
                            .Include(x => x.TipoAtividade)
                            .ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Atividades
                            .Include(x => x.Responsavel)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Atividade atividade)
        {
            try
            {
                db.Atividades.Add(atividade);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(atividade);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Atividade newItem)
        {
            var item = db.Atividades.Single(x => x.Id == newItem.Id);

            item.Nome = newItem.Nome;
            item.Entrega = newItem.Entrega;
            item.Tempo = newItem.Tempo;
            item.TipoImpressao = newItem.TipoImpressao;
            item.Observacao = newItem.Observacao;
            item.TipoAtividadeId = newItem.TipoAtividadeId;
            item.ResponsavelId = newItem.ResponsavelId;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Atividades.Single(x => x.Id == id);

            if (item.Tempo != null || !string.IsNullOrEmpty(item.Observacao) || item.Entrega != null)
                return BadRequest("Atividade não pode ser excluída");
            
            db.Atividades.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }

        [HttpGet("responsavel/{id}")]
        public IActionResult GetResponsavel(int id)
        {
            var item = db.Users.Select(x => new KeyValuePair<string, string>(x.Id, x.Name));
            return Ok(item);
        }

        [HttpGet("tipoatividade")]
        public IActionResult GetTipoAtividade()
        {
            return Ok(db.TipoAtividades.ToList());
        }
    }
}
