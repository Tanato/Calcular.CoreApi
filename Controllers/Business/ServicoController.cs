using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ServicoController : Controller
    {
        private readonly ApplicationDbContext db;

        public ServicoController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Servicos
                            .Include(x => x.Processo)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .Where(x => string.IsNullOrEmpty(filter)
                                        || x.Processo.Numero.Contains(filter)
                                        || x.Processo.Advogado.Nome.Contains(filter)
                                        || x.Processo.Advogado.Telefone.Contains(filter)
                                        || x.Processo.Advogado.Celular.Contains(filter))
                                        .ToList();

            return Ok(result);
        }

        [HttpGet]
        [Route("numero")]
        public IActionResult GetByNumber([FromQuery] string filter)
        {
            var result = db.Servicos
                            .Include(x => x.Processo)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || x.Processo.Numero.Contains(filter))
                            .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Servicos
                            .Include(x => x.Processo)
                            .Include(x => x.Atividades).ThenInclude(x => x.Responsavel)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Servico servico)
        {
            try
            {
                db.Servicos.Add(servico);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(servico);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Servico newItem)
        {
            var item = db.Servicos.Single(x => x.Id == newItem.Id);

            item.Entrada = newItem.Entrada;
            item.Prazo = newItem.Prazo;
            item.Saida = newItem.Saida;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Servicos.Single(x => x.Id == id);
            db.Servicos.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }
    }
}
