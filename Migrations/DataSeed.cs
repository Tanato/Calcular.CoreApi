using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
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
                await roleManager.CreateAsync(new IdentityRole { Name = "Associado" });
                await roleManager.CreateAsync(new IdentityRole { Name = "Inativo" });
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

            if (!context.TipoAtividades.Any())
            {
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Atualização" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Cálculo + Parecer" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Cálculo Assistência" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Cálculo Oficial" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Cálculo Oficial + Quesitos" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Esclarecimento" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Impugnação" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Levantamento Assistência" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Levantamento Oficial" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Levantamento Oficial + Quesitos" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Parecer" });
                context.TipoAtividades.Add(new TipoAtividade { Nome = "Saldo Remanescente" });
            }

            context.SaveChanges();
        }
    }
}
