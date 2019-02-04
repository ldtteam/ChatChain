using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Extensions;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer_WebApp.Data;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace IdentityServer_WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var redisConnectionVariable = Environment.GetEnvironmentVariable("REDIS_STACK_EXCHANGE");
            
            if (redisConnectionVariable != null && !redisConnectionVariable.IsNullOrEmpty())
            {
                var redis = ConnectionMultiplexer.Connect(redisConnectionVariable);
                services.AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
                    .SetApplicationName("WebApp");
            }
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var identityDatabase = Environment.GetEnvironmentVariable("IDENTITY_DATABASE");

            if (identityDatabase != null && !identityDatabase.IsNullOrEmpty())
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(
                        identityDatabase));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseMySql(
                        Configuration.GetConnectionString("IdentityDatabase")));
            }
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            
            ConfigurationStoreOptions cso = new ConfigurationStoreOptions();
            
            var identityServerDatabase = Environment.GetEnvironmentVariable("IDENTITY_SERVER_DATABASE");

            if (identityServerDatabase != null && !identityServerDatabase.IsNullOrEmpty() )
            {
                cso.ConfigureDbContext = builder =>
                {
                    builder.UseMySql(
                        identityServerDatabase,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                };
                services.AddSingleton(cso);
                services.AddDbContext<ConfigurationDbContext>(
                    builder =>
                    {
                        builder.UseMySql(
                            identityServerDatabase,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    });
            }
            else
            {
                cso.ConfigureDbContext = builder =>
                {
                    builder.UseMySql(
                        Configuration.GetConnectionString("IdentityServerDatabase"),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                };
                services.AddSingleton(cso);
                services.AddDbContext<ConfigurationDbContext>(
                    builder =>
                    {
                        builder.UseMySql(
                            Configuration.GetConnectionString("IdentityServerDatabase"),
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    });
                
            }
            
            services.AddDefaultIdentity<IdentityUser>(options => options.Password.RequireNonAlphanumeric = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<ClientService>();
            services.AddScoped<GroupService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            UpdateDatabase(app);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            var useHttps = Environment.GetEnvironmentVariable("USE_HTTPS");

            if (useHttps != null && !useHttps.IsNullOrEmpty())
            {
                var boolUseHttps = bool.Parse(useHttps);

                if (boolUseHttps)
                {
                    app.UseHttpsRedirection();
                }
            }
            
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }
        
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.EnsureCreated();
                    //context.Database.Migrate();
                }
            }
        }
    }
}