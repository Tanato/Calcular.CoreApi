using Calcular.CoreApi.Common;
using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ProcessoController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<User> userManager;

        public ProcessoController(UserManager<User> userManager, ApplicationDbContext db)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || x.Numero.Contains(filter)
                                             || x.Reu.Contains(filter)
                                             || x.Autor.Contains(filter)
                                             || x.Advogado.Nome.Contains(filter));

            return Ok(result.ToList());
        }

        [HttpGet]
        [Route("numero")]
        public IActionResult GetByNumber([FromQuery] string filter)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.ProcessoDetalhes).ThenInclude(x => x.User)
                            .Where(x => string.IsNullOrEmpty(filter)
                                             || x.Numero.Contains(filter))
                            .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Processos
                            .Include(x => x.Advogado)
                            .Include(x => x.Honorarios)
                            .Include(x => x.ProcessoDetalhes).ThenInclude(x => x.User)
                            .SingleOrDefault(x => x.Id == id);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostProcesso([FromBody] Processo processo)
        {
            try
            {
                db.Processos.Add(processo);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(processo);
        }

        [HttpPost]
        [Route("detalhe")]
        public IActionResult PostProcessoDetalhe([FromBody] ProcessoDetalhe processoDetalhe)
        {
            try
            {
                var user = userManager.GetUserAsync(HttpContext.User).Result;
                processoDetalhe.UserId = user.Id;
                processoDetalhe.Data = DateTime.Now;

                var processo = db.Processos
                                .Include(x => x.ProcessoDetalhes)
                                .Single(x => x.Id == processoDetalhe.ProcessoId);

                processo.ProcessoDetalhes.Add(processoDetalhe);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
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
            item.Vara = newItem.Vara;
            item.AdvogadoId = newItem.AdvogadoId;
            item.IndicacaoId = newItem.IndicacaoId;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProcesso(int id)
        {
            var item = db.Processos
                        .Include(x => x.Honorarios)
                        .Include(x => x.Servicos)
                        .Single(x => x.Id == id);

            if (item.Honorarios?.Count > 0 || item.Servicos?.Count > 0)
            {
                return BadRequest("O processo possúi Honorários/Serviços associados e não pode ser excluído");
            }
            else
            {
                db.Processos.Remove(item);
                db.SaveChanges();
                return Ok(item);
            }
        }

        [Route("local")]
        [HttpGet]
        public IActionResult GetLocal()
        {
            var result = Enum.GetValues(typeof(LocalEnum))
                            .Cast<LocalEnum>()
                            .Select(x => new { Key = (int)x, Value = EnumHelpers.GetEnumDescription(x), Mask = EnumHelpers.GetAttributeOfType<MaskAttribute>(x) });
            return Ok(result);
        }

        [Route("parte")]
        [HttpGet]
        public IActionResult GetParte()
        {
            var result = Enum.GetValues(typeof(ParteEnum))
                            .Cast<ParteEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [HttpGet("vara/{filter?}")]
        public IActionResult GetVara([FromQuery] string filter = "")
        {
            var result = db.Processos.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Vara.ContainsIgnoreNonSpacing(filter))
                                    .Select(x => x.Vara)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

            return Ok(result);
        }
    }
}
