using System;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerRepository;
using ChatChainCommon.IdentityServerStore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMongodb(this IServiceCollection services, Action<MongoConnections> mongoConnectionsAction)
        {
            MongoConnections mongoConnections = new MongoConnections();
            mongoConnectionsAction.Invoke(mongoConnections);
            services.AddSingleton(mongoConnections);
            
            services.AddScoped<ClientService>();
            services.AddScoped<GroupService>();
            services.AddScoped<ClientConfigService>();
            services.AddScoped<OrganisationService>();
            
            services.AddTransient<IRepository, MongoRepository>();
            services.AddScoped<CustomClientStore>();

            return services;
        }
    }
}