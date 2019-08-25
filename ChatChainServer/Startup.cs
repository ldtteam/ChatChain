using ChatChainCommon.Config;
using ChatChainCommon.DatabaseServices;
using ChatChainServer.Hubs;
using ChatChainServer.Utils;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;

namespace ChatChainServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables(options => { options.Prefix = "ChatChain_Server_"; });
            _configuration = builder.Build();
        }

        private readonly IConfigurationRoot _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = 
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            
            services.AddMvc();

            IdentityModelEventSource.ShowPII = true;
            
            IdentityServerConnection identityServerConnection = new IdentityServerConnection();
            _configuration.GetSection("IdentityServerConnection").Bind(identityServerConnection);

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

            services.AddSingleton<IUserIdProvider, ChatChainUserProvider>();
            
            MongoConnections mongoConnections = new MongoConnections();
            _configuration.GetSection("MongoConnections").Bind(mongoConnections);
            services.AddSingleton(mongoConnections);
            
            services.AddScoped<ClientService>();
            services.AddScoped<GroupService>();
            services.AddScoped<ClientConfigService>();
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

            app.UseStaticFiles();
            app.UseCookiePolicy();

            bool useHttps = _configuration.GetValue<bool>("UseHttps");

            if (useHttps)
            {
                    app.UseHttpsRedirection();
            }

            app.UseMvc();
            
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatChainHub>("/hubs/chatchain");
            });
        }
    }
}
