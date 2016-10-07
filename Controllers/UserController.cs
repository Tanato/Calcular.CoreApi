using System.Collections.Generic;
using System.Linq;
using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
            var users = db.Users.Include(x => x.Roles);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = db.Users.Include(x => x.Roles).SingleOrDefault(x => x.Id.Equals(id));
            return Ok(new { Name = user.Name, BirthDate = user.BirthDate, Email = user.Email, Login = user.UserName });
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
