using System.Collections.Generic;
using System.Linq;
using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Calcular.CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext db;

        public ProfileController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok(db.Profiles);
        }

        [HttpGet("{id}")]
        public Profile GetProfileById(int id)
        {
            return db.Profiles.Include(x => x.UserProfiles).ThenInclude(x => x.User)
                              .SingleOrDefault(x => x.Id == id);
        }

        [HttpPost]
        public void Post([FromBody]Profile profile)
        {
            db.Add(profile);
            db.SaveChanges();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var profile = db.Profiles.SingleOrDefault(x => x.Id == id);
            if (profile != null)
            {
                db.Profiles.Remove(profile);
                db.SaveChanges();
            }
        }
    }
}