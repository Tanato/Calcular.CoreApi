using System.Collections.Generic;
using System.Linq;
using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calcular.CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db;

        public UsersController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = db.Users.Include(x => x.UserProfiles).ThenInclude(x => x.Profile)
                                .Select(x => new
                                {
                                    Name = x.Name,
                                    Id = x.Id,
                                    Profiles = x.UserProfiles.Select(z => z.Profile)
                                });
            return Ok(users);
        }

        [HttpGet("{id}")]
        public User Get(int id)
        {
            return db.Users.SingleOrDefault(x => x.Id == id);
        }

        [HttpPost]
        public void Post([FromBody]User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var user = db.Users.SingleOrDefault(x => x.Id == id);
            if (true)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }
    }
}
