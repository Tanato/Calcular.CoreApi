using System.Collections.Generic;
using System.Linq;
using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Calcular.CoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext db;

        public RoleController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok(db.Roles);
        }

        [HttpGet("{id}")]
        public ActionResult GetRoleByUserId(string id)
        {
            return Ok(db.Roles.Include(x => x.Users).Where(x => x.Id == x.Users.SingleOrDefault(z => z.UserId.Equals(id)).RoleId));
        }
    }
}