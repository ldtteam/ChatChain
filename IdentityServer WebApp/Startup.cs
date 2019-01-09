using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer_WebApp.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<GroupsDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("GroupsDatabase")));
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            
            ConfigurationStoreOptions cso = new ConfigurationStoreOptions();

            cso.ConfigureDbContext = builder =>
            {
                builder.UseSqlite(
                    Configuration.GetConnectionString("IdentityServer"),
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            };
            
            services.AddSingleton(cso);

            services.AddDbContext<ConfigurationDbContext>(
                builder =>
                {
                    builder.UseSqlite(
                        Configuration.GetConnectionString("IdentityServer"),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                });
            
            services.AddDefaultIdentity<IdentityUser>(options => options.Password.RequireNonAlphanumeric = false )
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var factory = new ConnectionFactory {HostName = "66.70.180.4"};
            
            services.AddSingleton<IConnectionFactory>(factory);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}