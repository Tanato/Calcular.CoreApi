using System.Collections.Generic;
using System.Linq;
using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calcular.CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly PasswordHasher<User> pwdHasher = new PasswordHasher<User>();
        private readonly ApplicationDbContext db;

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
            return db.Users.Include(x => x.UserProfiles).ThenInclude(x => x.Profile)
                           .SingleOrDefault(x => x.Id == id);
        }

        [HttpPost]
        public void Post([FromBody]User user)
        {
            user.Password = pwdHasher.HashPassword(user, user.Password);
            db.Users.Add(user);
            db.SaveChanges();
        }

        [HttpPost("{id}")]
        public void AddProfileToUser(int id, [FromBody]IEnumerable<int> profileIds)
        {
            var user = db.Users.Include(x => x.UserProfiles).Single(x => x.Id == id);
            var profiles = db.Profiles.Where(x => !profileIds.Contains(x.Id));
            user.UserProfiles.AddRange(profiles.Select(x => new UserProfile { ProfileId = x.Id, UserId = user.Id }));
            db.SaveChanges();
        }


        [HttpPut("{id}")]
        public void Put(int id, [FromBody]User user)
        {
            var userDb = db.Users.SingleOrDefault(x => x.Id == id);
            if (userDb != null)
            {
                userDb.Name = user.Name;
                userDb.Email = user.Email;
            }
        }

        [HttpPut("password/{id}/{password}/{newpassword}")]
        public void PutPassword(int id, string password, string newpassword)
        {
            var userDb = db.Users.SingleOrDefault(x => x.Id == id);
            if (userDb != null)
            {
                var verification = pwdHasher.VerifyHashedPassword(userDb, userDb.Password, password);
                if (verification.Equals(PasswordVerificationResult.Success))
                {
                    userDb.Password = pwdHasher.HashPassword(userDb, newpassword);
                    db.SaveChanges();
                }
            }
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var user = db.Users.SingleOrDefault(x => x.Id == id);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }
    }
}
