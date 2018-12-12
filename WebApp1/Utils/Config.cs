using System.Collections.Generic;
using IdentityServer4.Models;

namespace WebApp1.Utils
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

                    RequireConsent = false,
                    
                    //Client secrets
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        "api1"
                    }
                }
            };
        }
        
        //API that are allowed to access the Auth server
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "Api 1")
            };
        }

    }
}