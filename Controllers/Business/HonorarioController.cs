using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class HonorarioController : Controller
    {
        private readonly ApplicationDbContext db;

        public HonorarioController(ApplicationDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Não utilizado no grid principal de Registro de Honorários, usa o de Processos
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Honorarios.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Processo.Numero.Contains(filter)
                                             || x.Processo.Advogado.Nome.Contains(filter)
                                             || x.Processo.Advogado.Telefone.Contains(filter)
                                             || x.Processo.Advogado.Celular.Contains(filter));

            return Ok(result.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Honorarios.SingleOrDefault(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Honorario honorario)
        {
            try
            {
                db.Honorarios.Add(honorario);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(honorario);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Honorario newItem)
        {
            var item = db.Honorarios.Single(x => x.Id == newItem.Id);

            item.NotaFiscal = newItem.NotaFiscal;
            item.Prazo = newItem.Prazo;
            item.Registro = newItem.Registro;
            item.TipoPagamento = newItem.TipoPagamento;
            item.ProcessoId = newItem.ProcessoId;
            item.Valor = newItem.Valor;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = db.Honorarios.Single(x => x.Id == id);
            db.Honorarios.Remove(item);
            db.SaveChanges();
            return Ok(item);
        }

        [Route("registro")]
        [HttpGet]
        public IActionResult GetLocal()
        {
            var result = Enum.GetValues(typeof(RegistroEnum))
                            .Cast<RegistroEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [Route("tipopagamento/{registro}")]
        [HttpGet]
        public IActionResult GetParte(RegistroEnum registro)
        {
            switch (registro)
            {
                case RegistroEnum.Honorario:
                    return Ok(Enum.GetValues(typeof(TipoRegistroEnum))
                                 .Cast<TipoRegistroEnum>()
                                 .Where(x => (int)x <= 3)
                                 .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x))));
                case RegistroEnum.Pagamento:
                    return Ok(Enum.GetValues(typeof(TipoRegistroEnum))
                                .Cast<TipoRegistroEnum>()
                                 .Where(x => (int)x > 3)
                                .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x))));
                default:
                    return BadRequest();
            }
        }
    }
}
