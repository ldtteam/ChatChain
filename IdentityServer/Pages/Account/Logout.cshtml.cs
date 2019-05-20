using IdentityModel;
using IdentityServer.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Pages.Account
{
    public class Logout : PageModel
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<Login> _logger;
        private readonly IIdentityServerInteractionService _interaction;

        public Logout(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<Login> logger, IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _interaction = interaction;
        }
        
        public async void OnPostAsync(string logoutId)
        {
            
            if (logoutId.IsNullOrEmpty()) return;

            if (User?.Identity.IsAuthenticated != true) return;

            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            string externalAuthenticationScheme = null;

            if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
            {
                var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                if (providerSupportsSignout)
                {
                    externalAuthenticationScheme = idp;
                }
            }

            await _signInManager.SignOutAsync();

            if (externalAuthenticationScheme != null)
            {
                SignOut(externalAuthenticationScheme);
            }
        }
    }
}