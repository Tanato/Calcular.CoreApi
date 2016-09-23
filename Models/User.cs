using System;
using System.Collections.Generic;

namespace Calcular.CoreApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }

        public List<UserProfile> UserProfiles { get; set; }
    }
}