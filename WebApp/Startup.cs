using System;
using AspNetCore.Identity.Mongo;
using IdentityServer.Store;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repository;
using WebApp.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using StackExchange.Redis;

namespace WebApp
{
    public class Startup
    {
        //quick test for CI -- 3
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

            var emailUsername = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
            if (!emailUsername.IsNullOrEmpty())
            {
                var emailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
                var emailHost = Environment.GetEnvironmentVariable("EMAIL_HOST");
                var emailPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT"));
                var emailSsl = bool.Parse(Environment.GetEnvironmentVariable("EMAIL_ENABLE_SSL"));

                services.AddTransient<IEmailSender, EmailSender>(i =>
                    new EmailSender(emailHost, emailPort, emailSsl, emailUsername, emailPassword));
            }

            var identityDatabase = Environment.GetEnvironmentVariable("IDENTITY_DATABASE");

            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(identityOptions =>
            {
                identityOptions.Password.RequireNonAlphanumeric = false;
                /*if (!emailUsername.IsNullOrEmpty())
                {
                    identityOptions.SignIn.RequireConfirmedEmail = true;
                }*/
            }, mongoIdentityOptions => { mongoIdentityOptions.ConnectionString = identityDatabase; });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<ClientService>();
            services.AddScoped<GroupService>();
            services.AddScoped<ClientConfigService>();

            services.AddTransient<IRepository, MongoRepository>();
            services.AddScoped<CustomClientStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //UpdateDatabase(app);
            
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

            ConfigureMongoDriver2IgnoreExtraElements();
            
            //InitializeDatabase(app);
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<IdentityServer4.Models.Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
