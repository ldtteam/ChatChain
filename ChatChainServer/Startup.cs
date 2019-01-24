using System;
using System.Collections.Generic;
using ChatChainServer.Hubs;
using ChatChainServer.Services;
using ChatChainServer.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;

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
                options.ApiName = "api1";
            });
            
            services.AddSignalR();

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

            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatChainHub>("/hubs/chatchain");
            });
        }
    }
}
