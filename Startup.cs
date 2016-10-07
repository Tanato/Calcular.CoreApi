using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Calcular.CoreApi.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using Calcular.CoreApi.Migrations;
using Newtonsoft.Json.Linq;
using System.Linq;
using Steeltoe.Extensions.Configuration;
using Swashbuckle.Swagger.Model;

namespace Calcular.CoreApi
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        private IHostingEnvironment CurrentEnvironment { get; set; }

        public Startup(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
                builder.AddUserSecrets();
            else
                builder.AddCloudFoundry();

            Configuration = builder.Build();
            CurrentEnvironment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(GetConnectionString()));

            services.AddCors(options =>
                options.AddPolicy("AllowAll", p =>
                    p.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials()));

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Cookies.ApplicationCookie.ExpireTimeSpan = new TimeSpan(12, 0, 0);
                options.Cookies.ApplicationCookie.AutomaticChallenge = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.FromResult(0);
                    }
                };
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc(config =>
                {
                    config.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.EnsureSeedIdentity();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("AllowAll");

            app.UseIdentity();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();
        }

        #region Private Methods

        private string GetConnectionString()
        {
            string cfUserDatabaseCredentials = !CurrentEnvironment.IsDevelopment() ? "vcap:services:user-provided:0:credentials" : string.Empty;

            return string.Format(Configuration["ConnectionStrings:DefaultConnection"],
                                 Configuration[cfUserDatabaseCredentials + "server"],
                                 Configuration[cfUserDatabaseCredentials + "database"],
                                 Configuration[cfUserDatabaseCredentials + "user"],
                                 Configuration[cfUserDatabaseCredentials + "password"]);
        }
        #endregion
    }
}
