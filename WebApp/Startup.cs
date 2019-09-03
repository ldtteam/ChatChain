using System;
using System.IdentityModel.Tokens.Jwt;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerRepository;
using ChatChainCommon.IdentityServerStore;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StackExchange.Redis;

namespace WebApp
{
    public class Startup
    {
        //quick test for CI -- 5
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables(options => { options.Prefix = "ChatChain_WebApp_"; });
            _configuration = builder.Build();
        }

        private readonly IConfigurationRoot _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_configuration);
            
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });
            
            string redisConnectionVariable = _configuration.GetValue<string>("RedisConnection");

            if (redisConnectionVariable != null && !redisConnectionVariable.IsNullOrEmpty())
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionVariable);
                services.AddSingleton<IConnectionMultiplexer>(redis);
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

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            
            IdentityServerConnection identityServerConnection = new IdentityServerConnection();
            _configuration.GetSection("IdentityServerConnection").Bind(identityServerConnection);
            services.AddSingleton(identityServerConnection);

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = identityServerConnection.ServerUrl;
                    options.RequireHttpsMetadata = false;

                    options.ClientId = identityServerConnection.ClientId;
                    options.ClientSecret = identityServerConnection.ClientSecret;
                    options.SaveTokens = true;
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                
                options.LoginPath = "/Account/Login";
                //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                /*.AddRazorPagesOptions(options =>
                    {
                        options.Conventions.AddPageRoute("/Organisations/View/{organisation}", "/Organisations/{organisation}/View");
                    })*/;

            MongoConnections mongoConnections = new MongoConnections();
            _configuration.GetSection("MongoConnections").Bind(mongoConnections);
            services.AddSingleton(mongoConnections);
            
            services.AddScoped<ClientService>();
            services.AddScoped<GroupService>();
            services.AddScoped<ClientConfigService>();
            services.AddScoped<OrganisationService>();

            services.AddTransient<IRepository, MongoRepository>();
            services.AddScoped<CustomClientStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedProto
                });
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

            bool useHttps = _configuration.GetValue<bool>("UseHttps");

            if (useHttps)
            {
                app.UseHttpsRedirection();
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
