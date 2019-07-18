using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Account
{
    public class Logout : PageModel
    {
        
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;

        public Logout(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction)
        {
            _signInManager = signInManager;
            _interaction = interaction;
        }

        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            if (logoutId.IsNullOrEmpty())
            {
                await _signInManager.SignOutAsync();
                return LocalRedirect("/Index");
            }

            await _signInManager.SignOutAsync();

            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            if (logout.PostLogoutRedirectUri.IsNullOrEmpty()) return null;
            
            return Redirect(logout.PostLogoutRedirectUri);
        }
    }
}