using System;
using System.Reflection;
using Api.Core;
using Api.Infrastructure;
using Api.Infrastructure.Data.Mapping;
using Api.Infrastructure.Data.MongoDB;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using ChatChainCommon.Config;
using Hub.Core;
using Hub.Hubs;
using Hub.Utils;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hub
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables(options => { options.Prefix = "ChatChain_Hub_"; });
            _configuration = builder.Build();
        }

        private readonly IConfigurationRoot _configuration;
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = 
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            IdentityServerConnection identityServerConnection = new IdentityServerConnection();
            _configuration.GetSection("IdentityServerConnection").Bind(identityServerConnection);
            services.AddSingleton(identityServerConnection);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = identityServerConnection.ServerUrl;
                options.TokenRetriever = CustomTokenRetriever.FromHeaderAndQueryString;
                options.RequireHttpsMetadata = false;
                options.ApiName = "ChatChain";
            });
            
            string redisConnectionVariable = _configuration.GetValue<string>("RedisConnection");

            if (redisConnectionVariable != null && !redisConnectionVariable.IsNullOrEmpty())
            {
                services.AddSignalR().AddStackExchangeRedis(redisConnectionVariable, options =>
                {
                    options.Configuration.ChannelPrefix = "ChatChain";
                });
            }
            else
            {
                services.AddSignalR();
            }
            
            services.AddSingleton<IUserIdProvider, UserProvider>();
            
            services.AddAutoMapper(typeof(DataProfile));

            MongoConnectionOptions mongoConnectionOptions = new MongoConnectionOptions();
            _configuration.GetSection(nameof(MongoConnectionOptions)).Bind(mongoConnectionOptions);
            services.AddSingleton(mongoConnectionOptions);

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new HubCoreModule());
            builder.RegisterModule(new InfrastructureModule());

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Presenter"))
                .SingleInstance();
            
            builder.Populate(services);
            
            IContainer container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatChainHub>("/hubs/chatchain");
            });
        }
    }
}