using Calcular.CoreApi.Migrations;
using Calcular.CoreApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Steeltoe.Extensions.Configuration;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

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
            if (CurrentEnvironment.IsDevelopment())
            {
                var sqlconn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Calcular.CoreSql;Integrated Security=True";
                services.AddEntityFrameworkSqlServer();
                services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(sqlconn), ServiceLifetime.Transient);
            }
            else
            {
                Console.WriteLine(GetConnectionString());

                services.AddEntityFrameworkSqlServer();
                services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(GetConnectionString()), ServiceLifetime.Transient);
            }

            services.AddCors(options =>
                options.AddPolicy("CorsPolicy", p =>
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

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });

            services.AddResponseCaching(x => { x.MaximumBodySize = 1000000; });

            services.AddMvc(config =>
            {
                config.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));

                config.CacheProfiles.Add("Default",
                new CacheProfile()
                {
                    Duration = 60
                });
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-ddThh:mm:ss-03:00";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use(async (context, next) =>
            {
                await next.Invoke();
                if (new Random().Next(2) == 0)
                    GC.Collect();
            });

            app.UseResponseCompression();
            app.UseResponseCaching();

            app.EnsureSeedIdentityAsync();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("CorsPolicy");

            app.UseIdentity();

            app.UseExceptionHandler();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        #region Private Methods

        private string GetConnectionString()
        {
            string cfUserDatabaseCredentials = !CurrentEnvironment.IsDevelopment() ? "vcap:services:user-provided:0:credentials:" : string.Empty;

            return string.Format(Configuration["ConnectionStrings:DefaultConnection"],
                                 Configuration[cfUserDatabaseCredentials + "server"],
                                 Configuration[cfUserDatabaseCredentials + "database"],
                                 Configuration[cfUserDatabaseCredentials + "user"],
                                 Configuration[cfUserDatabaseCredentials + "password"]);
        }
        #endregion
    }
}
