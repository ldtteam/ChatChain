using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Services
{
    public class UserClaimsPrincipleFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public UserClaimsPrincipleFactory(UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) 
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("DisplayName", user.DisplayName ?? " "));
            return identity;
        }
    }
}