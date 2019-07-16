using System.Collections.Generic;
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

    }
}