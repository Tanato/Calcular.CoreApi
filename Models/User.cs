using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Calcular.CoreApi.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
    }
}