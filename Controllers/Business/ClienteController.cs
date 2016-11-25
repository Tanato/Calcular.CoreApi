using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.ViewModels;
using Calcular.CoreApi.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext db;

        public ClienteController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Clientes.Where(x => string.IsNullOrEmpty(filter)
                                             || (!string.IsNullOrEmpty(x.Nome) && x.Nome.ContainsIgnoreNonSpacing(filter))
                                             || (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(filter))
                                             || (!string.IsNullOrEmpty(x.Empresa) && x.Empresa.ContainsIgnoreNonSpacing(filter))
                                             || (!string.IsNullOrEmpty(x.Celular) && x.Celular.Contains(filter)))
                                    .OrderBy(x => x.Nome);

            return Ok(result.ToList());
        }

        [HttpGet("select")]
        public IActionResult GetKeyValue([FromQuery] string filter)
        {
            var result = db.Clientes.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Nome.ContainsIgnoreNonSpacing(filter))
                                    .OrderBy(x => x.Nome)
                                    .Select(x => new KeyValuePair<int, string>(x.Id, x.Nome)).ToList();

            return Ok(result);
        }

        [HttpGet("select/{id}")]
        public IActionResult GetKeyValue(int id)
        {
            var result = db.Clientes.Select(x => new KeyValuePair<int, string>(x.Id, x.Nome)).Single(x => x.Key == id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = db.Clientes.Single(x => x.Id == id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostCliente([FromBody] Cliente cliente)
        {
            cliente.Nascimento = cliente.Nascimento?.Date.AddHours(12);
            try
            {
                db.Clientes.Add(cliente);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(cliente);
        }

        [HttpPut]
        public IActionResult PutCliente([FromBody] Cliente newItem)
        {
            var item = db.Clientes.Single(x => x.Id == newItem.Id);

            item.Nome = newItem.Nome;
            item.Email = newItem.Email;
            item.Endereco = newItem.Endereco;
            item.Telefone = newItem.Telefone;
            item.Telefone2 = newItem.Telefone2;
            item.Celular = newItem.Celular;
            item.Celular2 = newItem.Celular2;
            item.Nascimento = newItem.Nascimento?.Date.AddHours(12);
            item.Perfil = newItem.Perfil;
            item.Empresa = newItem.Empresa;
            item.Honorarios = newItem.Honorarios;
            item.ComoChegou = newItem.ComoChegou;
            item.ComoChegouDetalhe = newItem.ComoChegouDetalhe;
            item.Observacao = newItem.Observacao;

            db.SaveChanges();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCliente(int id)
        {
            var item = db.Clientes
                        .Include(x => x.Processos)
                        .Include(x => x.Honorarios)
                        .Single(x => x.Id == id);

            if (item.Processos?.Count > 0)
            {
                return BadRequest("O processo possúi Honorários/Serviços associados e não pode ser excluído");
            }
            else
            {
                db.Clientes.Remove(item);
                db.SaveChanges();
                return Ok(item);
            }
        }

        [Route("perfil")]
        [HttpGet]
        public IActionResult GetPerfil()
        {
            var result = Enum.GetValues(typeof(PerfilEnum))
                            .Cast<PerfilEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [Route("comochegou")]
        [HttpGet]
        public IActionResult GetComoChegou()
        {
            var result = Enum.GetValues(typeof(ComoChegouEnum))
                            .Cast<ComoChegouEnum>()
                            .Select(x => new KeyValuePair<int, string>((int)x, EnumHelpers.GetEnumDescription(x)));
            return Ok(result);
        }

        [HttpGet("company/{filter?}")]
        public IActionResult GetExistingCompany([FromQuery] string filter = "")
        {
            var result = db.Clientes.Where(x => string.IsNullOrEmpty(filter)
                                             || x.Empresa.ContainsIgnoreNonSpacing(filter))
                                    .Select(x => x.Empresa)
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

            return Ok(result);
        }
    }
}
