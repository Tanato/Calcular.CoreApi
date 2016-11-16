using System.Collections.Generic;
using System.Linq;
using Calcular.CoreApi.Models.Business;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Calcular.CoreApi.Models;

namespace Calcular.CoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private UserManager<User> userManager;
        private readonly ApplicationDbContext db;

        public UserController(ApplicationDbContext db, UserManager<User> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = db.Users.Include(x => x.Roles).OrderBy(x => x.UserName);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var user = db.Users.Include(x => x.Roles).SingleOrDefault(x => x.Id.Equals(id));
            if (user != null)
            {
                var result = new
                {
                    UserName = user.UserName,
                    Name = user.Name,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Login = user.UserName,
                    Roles = user.Roles.Select(x => x.RoleId),
                };
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("{userId}")]
        public void AddProfileToUser(string userId, [FromBody]IEnumerable<string> roles)
        {
            var user = db.Users.SingleOrDefault(x => x.Id.Equals(userId));
            userManager.AddToRolesAsync(user, roles);
            db.SaveChanges();
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]User user)
        {
            var userDb = db.Users.SingleOrDefault(x => x.Id.Equals(id));
            if (userDb != null)
            {
                userDb.Name = user.Name;
                userDb.Email = user.Email;
            }
        }
    }
}
