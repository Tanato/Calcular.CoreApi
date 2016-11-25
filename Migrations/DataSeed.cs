﻿using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Migrations
{
    public static class DataSeed
    {
        public static async void EnsureSeedIdentityAsync(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<ApplicationDbContext>();

            if (context != null)
            {
                context.Database.Migrate();
            }

            var roleManager = app.ApplicationServices.GetService<RoleManager<IdentityRole>>();
            var userManager = app.ApplicationServices.GetService<UserManager<User>>();

            if (!context.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole { Name = "Administrativo" });
                await roleManager.CreateAsync(new IdentityRole { Name = "Calculista" });
                await roleManager.CreateAsync(new IdentityRole { Name = "Revisor" });
                await roleManager.CreateAsync(new IdentityRole { Name = "Gerencial" });
            }

            if (!context.Users.Any())
            {
                var userResult = await userManager.CreateAsync(new User { Name = "Tanato Cartaxo", UserName = "Admin" }, "123Admin");

                if (userResult.Succeeded)
                {
                    var AdminUser = await userManager.FindByNameAsync("Admin");

                    if (AdminUser != null && roleManager.RoleExistsAsync("Admin").Result)
                    {
                        await userManager.AddToRolesAsync(AdminUser, new string[] { "Administrativo", "Calculista", "Revisor", "Gerencial" });
                    }
                }
            }
        }
    }
}
