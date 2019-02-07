using System;
using System.Collections.Generic;
using ChatChainServer.Hubs;
using ChatChainServer.Services;
using ChatChainServer.Utils;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using StackExchange.Redis;

namespace ChatChainServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static readonly Dictionary<string, List<string>> ClientIds = new Dictionary<string, List<string>>();
        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = Environment.GetEnvironmentVariable("IDENTITY_SERVER_URL"); //"http://host.docker.internal:5081";
                options.TokenRetriever = CustomTokenRetriever.FromHeaderAndQueryString;
                options.RequireHttpsMetadata = false;
                options.ApiName = "ChatChain";
            });

            var environmentConnectionString = Environment.GetEnvironmentVariable("REDIS_BACKPLANE");

            if (environmentConnectionString != null && !environmentConnectionString.IsNullOrEmpty())
            {
                services.AddSignalR().AddStackExchangeRedis(environmentConnectionString, options =>
                    {
                        options.Configuration.ChannelPrefix = "ChatChain";
                    });
            }
            else
            {
                services.AddSignalR();
            }

            services.AddSingleton<IUserIdProvider, ChatChainUserProvider>();
            
            services.AddScoped<ClientService>();
            services.AddScoped<GroupService>();
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

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            var useHttps = Environment.GetEnvironmentVariable("USE_HTTPS");

            if (useHttps != null && !useHttps.IsNullOrEmpty())
            {
                var boolUseHttps = bool.Parse(useHttps);

                if (boolUseHttps)
                {
                    app.UseHttpsRedirection();
                }
            }

            app.UseMvc();
            
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatChainHub>("/");
            });
        }
    }
}
