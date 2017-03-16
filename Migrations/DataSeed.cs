using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Shared;
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

            var roles = new[] { "Administrativo", "Calculista", "Revisor", "Gerencial", "Associado", "Colaborador Externo" };

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
                        await userManager.AddToRolesAsync(AdminUser, new string[] { "Administrativo", "Calculista", "Revisor", "Gerencial", "Colaborador Externo" });
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

            //foreach (var item in context.Processos.Include(x => x.Honorarios))
            //{
            //    if (item.Honorarios != null && item.Honorarios.Count(x => !x.Cancelado) > 0)
            //        if (item.Honorario.HasValue && item.Honorario == 0 && !item.Honorarios.Any(x => !x.Cancelado && x.Registro == RegistroEnum.Pagamento))
            //            item.StatusHonorario = StatusHonorarioEnum.Pendente;
            //        else
            //            item.StatusHonorario = item.Total <= 0 ? StatusHonorarioEnum.Pago : StatusHonorarioEnum.Pendente;
            //    else
            //        item.StatusHonorario = StatusHonorarioEnum.Undefined;


            //    item.PrazoHonorario = item.Honorarios != null && item.Honorarios.Count > 0 ? item.Honorarios.Where(x => x.Registro == RegistroEnum.Honorario && !x.Cancelado).Max(x => x.Prazo) : null;
            //}

            context.SaveChanges();
        }
    }
}
