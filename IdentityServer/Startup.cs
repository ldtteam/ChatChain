using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Data;
using IdentityServer.Extension;
using IdentityServer.Interface;
using IdentityServer.Models;
using IdentityServer.Utils;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using StackExchange.Redis;

namespace IdentityServer
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
                    .SetApplicationName("IdentityServer");
            }
            
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            
            services.AddMvc();

            /*var environmentConnectionString = Environment.GetEnvironmentVariable("IDENTITY_SERVER_DATABASE");
            var connectionString = Configuration.GetConnectionString("IdentityServerDatabase");

            if (environmentConnectionString != null && !environmentConnectionString.IsNullOrEmpty())
            {
                connectionString = environmentConnectionString;
            }*/

            services.AddIdentityServer(options =>
                {
                    options.IssuerUri = Environment.GetEnvironmentVariable("IDENTITY_SERVER_URL");
                    options.PublicOrigin = Environment.GetEnvironmentVariable("IDENTITY_SERVER_ORIGIN");
                })
                .AddDeveloperSigningCredential()
                /*.AddConfigurationStore(options =>
                    options.ConfigureDbContext = builder =>
                        builder.UseMySql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseMySql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });*/
                .AddMongoRepository()
                .AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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
            
            app.UseIdentityServer();

            app.UseMvc();

            ConfigureMongoDriver2IgnoreExtraElements();

            InitializeDatabase(app);
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

        }
        
        private void InitializeDatabase(IApplicationBuilder app)
        {
            bool createdNewRepository = false;
            var repository = app.ApplicationServices.GetService<IRepository>();

            //  --IdentityResource
            if (!repository.CollectionExists<IdentityResource>())
            {
                foreach (var res in Config.GetIdentityResources())
                {
                    repository.Add(res);
                }
                createdNewRepository = true;
            }


            //  --ApiResource
            if (!repository.CollectionExists<ApiResource>())
            {
                foreach (var api in Config.GetApis())
                {
                    repository.Add(api);
                }
                createdNewRepository = true;
            }

            // If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
            if (createdNewRepository)
            {
                var newRepositoryMsg = $"Mongo Repository created/populated! Please restart you website, so Mongo driver will be configured  to ignore Extra Elements.";
                throw new Exception(newRepositoryMsg);
            }
        }
    }
}