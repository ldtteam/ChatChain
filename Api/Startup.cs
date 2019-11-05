using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Api.Core;
using Api.Infrastructure;
using Api.Infrastructure.Data.Mapping;
using Api.Infrastructure.Data.MongoDB;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using ChatChainCommon.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;

namespace Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables(options => { options.Prefix = "ChatChain_Api_"; });
            _configuration = builder.Build();
        }

        private readonly IConfigurationRoot _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            string redisConnectionVariable = _configuration.GetValue<string>("RedisConnection");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionVariable);
            services.AddSingleton<IConnectionMultiplexer>(redis);
            
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                })
                .AddControllersAsServices();
            
            IdentityServerConnection identityServerConnection = new IdentityServerConnection();
            _configuration.GetSection("IdentityServerConnection").Bind(identityServerConnection);
            services.AddSingleton(identityServerConnection);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = identityServerConnection.ServerUrl;
                    options.RequireHttpsMetadata = false;
                    
                    options.Audience = "ChatChainAPI";
                });
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "ChatChain WebApp API",
                    Description = "ChatChain's WebApp API for client, group and organisation management"
                } );

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme, Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                
                Dictionary<string, IEnumerable<string>> security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] {} }
                };
                options.AddSecurityRequirement(security);
            });

            services.AddAutoMapper(typeof(DataProfile));

            MongoConnectionOptions mongoConnectionOptions = new MongoConnectionOptions();
            _configuration.GetSection(nameof(MongoConnectionOptions)).Bind(mongoConnectionOptions);
            services.AddSingleton(mongoConnectionOptions);

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new InfrastructureModule());

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Presenter"))
                .SingleInstance();
            
            builder.Populate(services);
            
            IContainer container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseMvc();
            
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatChain WebApp API V1");
                options.RoutePrefix = string.Empty;
            });
            
            ConfigureMongoDriver2IgnoreExtraElements();
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