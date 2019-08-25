using System;
using System.Collections.Generic;
using System.Linq;
using ChatChainCommon.Config;
using ChatChainCommon.Config.IdentityServer;
using ChatChainCommon.IdentityServerRepository;
using IdentityServer.Extension;
using IdentityServer.Models;
using IdentityServer.Services;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using StackExchange.Redis;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables(options => { options.Prefix = "ChatChain_IdentityServer_"; });
            _configuration = builder.Build();
        }

        private IConfigurationRoot _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_configuration);
            
            MongoConnections mongoConnections = new MongoConnections();
            _configuration.GetSection("MongoConnections").Bind(mongoConnections);
            services.AddSingleton(mongoConnections);
            
            Console.WriteLine($"TESTING: {mongoConnections.IdentityServerConnection.ConnectionString}");
            Console.WriteLine($"TESTING: {mongoConnections.IdentityServerConnection.DatabaseName}");
            Console.WriteLine($"TESTING: {mongoConnections.IdentityConnection.ConnectionString}");
            Console.WriteLine($"TESTING: {mongoConnections.IdentityConnection.DatabaseName}");
            
            EmailConnection emailConnection = new EmailConnection();
            if (_configuration.GetSection("EmailConnection").Exists())
            {
                _configuration.GetSection("EmailConnection").Bind(emailConnection);
                services.AddSingleton(emailConnection);
            }
            
            Console.WriteLine($"TESTING: {emailConnection.Host}");
            Console.WriteLine($"TESTING: {emailConnection.Port}");
            Console.WriteLine($"TESTING: {emailConnection.Ssl}");
            Console.WriteLine($"TESTING: {emailConnection.Username}");
            
            IdentityServerOptions identityServerOptions = new IdentityServerOptions();
            _configuration.GetSection("IdentityServerOptions").Bind(identityServerOptions);
            services.AddSingleton(identityServerOptions);
            
            Console.WriteLine($"TESTING: {identityServerOptions.ServerOrigin}");
            Console.WriteLine($"TESTING: {identityServerOptions.ServerUrl}");
            Console.WriteLine($"TESTING: {identityServerOptions.SigningPath}");

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = 
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            
            string redisConnectionVariable = _configuration.GetValue<string>("RedisConnection");
            
            Console.WriteLine($"TESTING: {redisConnectionVariable}");
            
            if (redisConnectionVariable != null && !redisConnectionVariable.IsNullOrEmpty())
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionVariable);
                services.AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
                    .SetApplicationName("IdentityServer");
            }
            
            if (emailConnection.Host != null)
            {
                string emailUsername = emailConnection.Username;
                string emailPassword = emailConnection.Password;
                string emailHost = emailConnection.Host;
                int emailPort = emailConnection.Port;
                bool emailSsl = emailConnection.Ssl;

                services.AddTransient<EmailSender, EmailSender>(i =>
                    new EmailSender(emailHost, emailPort, emailSsl, emailUsername, emailPassword));
            }
            
            services.AddMvc();

            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(identityOptions =>
            {
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireDigit = false;
                identityOptions.User.RequireUniqueEmail = true;
            }, mongoIdentityOptions =>
            {
                mongoIdentityOptions.ConnectionString = mongoConnections.IdentityConnection.ConnectionString;
                mongoIdentityOptions.DatabaseName = mongoConnections.IdentityConnection.DatabaseName;
            });

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipleFactory>();

            IIdentityServerBuilder identityServerBuilder = services.AddIdentityServer(options =>
                {
                    options.IssuerUri = identityServerOptions.ServerUrl;
                    options.PublicOrigin = identityServerOptions.ServerOrigin;
                })
                //.AddDeveloperSigningCredential()
                .AddMongoRepository()
                //.AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants()
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>();
            
            if (_configuration.GetSection("ClientsConfig").Exists())
            {
                ClientsConfig clientsConfig = new ClientsConfig();

                _configuration.GetSection("ClientsConfig").Bind(clientsConfig);
                services.AddSingleton(clientsConfig);

                List<Client> clients = new List<Client>();

                foreach (ClientConfig clientConfig in clientsConfig.ClientConfigs)
                {
                    Console.WriteLine($"TESTING: {clientConfig.ClientId}");
                    Console.WriteLine($"TESTING: {clientConfig.ClientName}");
                    Console.WriteLine($"TESTING: {clientConfig.RequireConsent}");
                    Console.WriteLine($"TESTING: {clientConfig.RedirectUris}");
                    Console.WriteLine($"TESTING: {clientConfig.PostLogoutRedirectUris}");
                    Console.WriteLine($"TESTING: {clientConfig.AllowedCorsOrigins}");
                    Console.WriteLine($"TESTING: {clientConfig.AllowedGrantTypes}");
                    Console.WriteLine($"TESTING: {clientConfig.Secrets}");
                    Console.WriteLine($"TESTING: {clientConfig.AllowedScopes}");
                    Console.WriteLine($"TESTING: {clientConfig.FrontChannelLogoutUri}");
                    clients.Add(new Client
                    {
                        ClientId = clientConfig.ClientId,
                        ClientName = clientConfig.ClientName,
                        RequireConsent = clientConfig.RequireConsent,
                        RedirectUris = clientConfig.RedirectUris,
                        PostLogoutRedirectUris = clientConfig.PostLogoutRedirectUris,
                        AllowedCorsOrigins = clientConfig.AllowedCorsOrigins,
                        AllowedGrantTypes = clientConfig.AllowedGrantTypes,
                        ClientSecrets = clientConfig.Secrets.Select(secret => new Secret(secret.Sha256())).ToList(),
                        AllowedScopes = clientConfig.AllowedScopes,
                        FrontChannelLogoutUri = clientConfig.FrontChannelLogoutUri
                    });
                }

                identityServerBuilder.AddClientsWithInMemory(clients);
            }

            if (identityServerOptions.SigningPassword == null || identityServerOptions.SigningPath == null)
            {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                identityServerBuilder.AddCertificateFromFile(identityServerOptions);
            }
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
            
            bool useHttps = _configuration.GetValue<bool>("UseHttps");

            if (useHttps)
            {
                app.UseHttpsRedirection();
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
        
        private static void InitializeDatabase(IApplicationBuilder app)
        {
            bool createdNewRepository = false;
            IRepository repository = app.ApplicationServices.GetService<IRepository>();

            //  --IdentityResource
            if (!repository.CollectionExists<IdentityResource>())
            {
                foreach (IdentityResource res in IdentityConfig.GetIdentityResources())
                {
                    repository.Add(res);
                }
                createdNewRepository = true;
            }

            //  --ApiResource
            if (!repository.CollectionExists<ApiResource>())
            {
                foreach (ApiResource api in IdentityConfig.GetApis())
                {
                    repository.Add(api);
                }
                createdNewRepository = true;
            }

            // If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
            if (!createdNewRepository) return;
            const string newRepositoryMsg = "Mongo Repository created/populated! Please restart you website, so Mongo driver will be configured  to ignore Extra Elements.";
            throw new Exception(newRepositoryMsg);
        }
    }
}