using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
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

        public string LogoutId;
        
        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            if (logoutId.IsNullOrEmpty()) return null;

            await _signInManager.SignOutAsync();

            var logout = await _interaction.GetLogoutContextAsync(logoutId);
            
            return Redirect(logout.PostLogoutRedirectUri);
        }
    }
}