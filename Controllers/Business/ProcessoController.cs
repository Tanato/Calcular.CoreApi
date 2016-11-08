using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ProcessoController : Controller
    {
        private readonly ApplicationDbContext db;

        public ProcessoController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Processos.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Numero.Contains(filter)
                                             || x.Reu.Contains(filter)
                                             || x.Autor.Contains(filter)
                                             || x.Advogado.Contains(filter));

            return Ok(result.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Processos.Single(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostProcesso([FromBody] Processo Processo)
        {
            try
            {
                db.Processos.Add(Processo);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(Processo);
        }

        [HttpPut]
        public IActionResult PutProcesso([FromBody] Processo newItem)
        {
            var item = db.Processos.Single(x => x.Id == newItem.Id);

            item.Numero = newItem.Numero;
            item.Autor = newItem.Autor;
            item.Reu = newItem.Reu;
            item.Local = newItem.Local;
            item.Parte = newItem.Parte;
            item.Advogado = newItem.Advogado;
            item.PeritoId = newItem.PeritoId;
            item.Honorario = newItem.Honorario;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProcesso(int id)
        {
            var item = db.Processos.Single(x => x.Id == id);
            db.Processos.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }

        [Route("local")]
        [HttpGet]
        public IActionResult GetLocal()
        {
            var result = Enum.GetValues(typeof(LocalEnum))
                            .Cast<LocalEnum>()
                            .Select(x => new { Id = (int)x, Text = EnumHelpers.GetEnumDescription(x) });
            return Ok(result);
        }

        [Route("parte")]
        [HttpGet]
        public IActionResult GetParte()
        {
            var result = Enum.GetValues(typeof(ParteEnum))
                            .Cast<ParteEnum>()
                            .Select(x => new { Id = (int)x, Text = EnumHelpers.GetEnumDescription(x) });
            return Ok(result);  
        }
    }
}
