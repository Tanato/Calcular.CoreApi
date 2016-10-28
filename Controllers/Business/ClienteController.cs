﻿using Calcular.CoreApi.Models;
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
                                             || x.Nome.Contains(filter)
                                             || x.Email.Contains(filter)
                                             || x.Empresa.Contains(filter)
                                             || x.Celular.Contains(filter));

            return Ok(result.ToList());
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
            cliente.Nascimento = cliente.Nascimento.Date.AddHours(12);
            db.Clientes.Add(cliente);
            db.SaveChanges();
            return Ok();
        }

        [HttpPut]
        public IActionResult PutCliente([FromBody] Cliente newItem)
        {
            var item = db.Clientes.Single(x => x.Id == newItem.Id);

            item.Nome = newItem.Nome;
            item.Email = newItem.Email;
            item.Endereco = newItem.Endereco;
            item.Telefone = newItem.Telefone;
            item.Celular = newItem.Celular;
            item.Nascimento = newItem.Nascimento.Date.AddHours(12);
            item.Perfil = newItem.Perfil;
            item.Empresa = newItem.Empresa;
            item.Honorarios = newItem.Honorarios;
            item.ComoChegou = newItem.ComoChegou;
            item.ComoChegouDetalhe = newItem.ComoChegouDetalhe;

            db.SaveChanges();
            return Ok(item);
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
    }
}