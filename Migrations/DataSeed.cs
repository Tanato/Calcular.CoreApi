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

            var roles = new[] {"Gerencial", "Administrativo", "Revisor", "Calculista", "Colaborador Externo" };

            foreach (var item in roles)
            {
                if (!context.Roles.Any(x => x.Name == item))
                    await roleManager.CreateAsync(new IdentityRole { Name = item });
            }

            if (!context.Users.Any())
            {
                var userResult = await userManager.CreateAsync(new User { Name = "Tanato Cartaxo", UserName = "Admin" }, "123Admin");

                if (userResult.Succeeded)
                {
                    var AdminUser = await userManager.FindByNameAsync("Admin");

                    if (AdminUser != null && roleManager.RoleExistsAsync("Admin").Result)
                    {
                        await userManager.AddToRolesAsync(AdminUser, new string[] { "Administrativo", "Calculista", "Revisor", "Gerencial", "Colaborador Externo"});
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
