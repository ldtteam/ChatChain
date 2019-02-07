using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer.Utils
{
    public class Config
    {
        //Clients that are allowed to access resources from the Auth server
        public static IEnumerable<Client> GetClients()
        {
            //Client credentials, list of clients
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    //Client secrets
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                       "api1"
                    },
                    AllowOfflineAccess = true
                }
            };
        }
        
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

    }
}