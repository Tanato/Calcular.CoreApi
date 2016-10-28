﻿using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class TipoAtividadeController : Controller
    {
        private readonly ApplicationDbContext db;

        public TipoAtividadeController(ApplicationDbContext db)
        {
            this.db = db;
        }
        
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(db.TipoAtividades.ToList());
        }
    }
}