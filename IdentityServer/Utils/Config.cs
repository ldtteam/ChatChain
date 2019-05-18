using System;
using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer.Utils
{
    public class Config
    {
        
        //API that are allowed to access the Auth server
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("ChatChain", "ChatChain API")
            };
        }
        
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "auth",
                    ClientName = "Auth Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    
                    RedirectUris = { Environment.GetEnvironmentVariable("WEBAPP_SERVER_URL") + "/signin-oidc" }, //TODO: REMOVE THIS
                    
                    PostLogoutRedirectUris = { Environment.GetEnvironmentVariable("WEBAPP_SERVER_URL") + "/signout-callback-oidc" }, //TODO: REMOVE THIS

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
        }

    }
}