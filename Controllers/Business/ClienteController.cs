using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public IActionResult GetAll()
        {
            return Ok(db.Clientes);
        }
    }
}
