using System;
using AspNetCore.Identity.Mongo.Collections;
using AspNetCore.Identity.Mongo.Model;
using AspNetCore.Identity.Mongo.Stores;
using ChatChainCommon.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Services
{
    public static class IdentityExtensions
    {
        public static IdentityBuilder AddIdentityMongoDbProvider<TUser, TRole>(this IServiceCollection services,
            Action<IdentityOptions> setupIdentityAction, Action<MongoOptions> setupDatabaseAction) where TUser : MongoUser
            where TRole : MongoRole
        {
            MongoOptions dbOptions = new MongoOptions();
            setupDatabaseAction(dbOptions);

            IdentityBuilder builder = services.AddIdentity<TUser, TRole>(setupIdentityAction ?? (x => { }));
	        
            builder.AddRoleStore<RoleStore<TRole>>()
                .AddUserStore<UserStore<TUser, TRole>>()
                .AddUserManager<UserManager<TUser>>()
                .AddRoleManager<RoleManager<TRole>>()
                .AddDefaultTokenProviders();

            IdentityUserCollection<TUser> userCollection = new IdentityUserCollection<TUser>(dbOptions.ConnectionString, dbOptions.DatabaseName, "Users");
            IdentityRoleCollection<TRole> roleCollection = new IdentityRoleCollection<TRole>(dbOptions.ConnectionString, dbOptions.DatabaseName, "Roles");

            services.AddTransient<IIdentityUserCollection<TUser>>(x => userCollection);
            services.AddTransient<IIdentityRoleCollection<TRole>>(x => roleCollection);

            // Identity Services
            services.AddTransient<IUserStore<TUser>>(x => new UserStore<TUser, TRole>(userCollection, roleCollection, x.GetService<ILookupNormalizer>()));
            services.AddTransient<IRoleStore<TRole>>(x => new RoleStore<TRole>(roleCollection));
	       
	        
            return builder;
        }
    }
}