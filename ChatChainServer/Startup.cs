using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatChainServer.Data;
using ChatChainServer.Hubs;
using ChatChainServer.Services;
using ChatChainServer.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
            /*services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });*/

            services.AddDbContext<GroupsDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("GroupsDatabase"))); 
            
            services.AddMvc();

            services.AddAuthentication(options =>
            {               
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "http://localhost:5001";
                options.TokenRetriever = CustomTokenRetriever.FromHeaderAndQueryString;
                options.RequireHttpsMetadata = false;
//                options.ApiSecret = "secret";
//                options.Authority = "client";
                options.ApiName = "api1";
            });
            
            services.AddSignalR();

            services.AddSingleton<IUserIdProvider, ChatChainUserProvider>();

            //services.AddHostedService<RabbitMqService>();
            //services.AddScoped<IRabbitMqService, ScopedRabbitMqService>();
            
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
