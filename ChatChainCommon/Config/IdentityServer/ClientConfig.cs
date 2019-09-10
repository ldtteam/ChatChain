using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace ChatChainCommon.Config.IdentityServer
{
    public class ClientConfig
    {
        public string ClientId { get; set; }
        
        public string ClientName { get; set; }

        public bool RequireConsent { get; set; } = false;
        
        public string FrontChannelLogoutUri { get; set; }
 
        public ICollection<string> RedirectUris { get; set; }
        
        public ICollection<string> PostLogoutRedirectUris { get; set; } 
        
        public ICollection<string> AllowedCorsOrigins { get; set; }

        public ICollection<string> AllowedGrantTypes { get; set; }// = GrantTypes.Implicit;
        
        public ICollection<string> Secrets { get; set; }

        public ICollection<string> AllowedScopes { get; set; } = new List<string>
        {
            IdentityServerConstants.StandardScopes.OpenId,
            IdentityServerConstants.StandardScopes.Profile,
            "ChatChainAPI"
        };
    }
}