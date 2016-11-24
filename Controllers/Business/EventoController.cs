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
    public class EventoController : Controller
    {
        private readonly ApplicationDbContext db;

        public EventoController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string filter)
        {
            var result = db.Users.Where(x => x.BirthDate.DayOfYear == DateTime.Now.DayOfYear);
            return Ok(result.ToList());
        }
    }
}
