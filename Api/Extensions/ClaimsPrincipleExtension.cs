using System.Linq;
using System.Security.Claims;

namespace Api.Extensions
{
    public static class ClaimsPrincipleExtension
    {
        public static string GetId(this ClaimsPrincipal principal)
        {
            return GetClaim(principal, "sub");
        }
        
        public static string GetClaim(this ClaimsPrincipal principal, string claimType)
        {
            return principal.Claims.First(claim => claim.Type.Equals(claimType)).Value;
        }
    }
}