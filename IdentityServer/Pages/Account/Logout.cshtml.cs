using System.Threading.Tasks;
using ChatChainCommon.Config.IdentityServer;
using IdentityServer.Models;
using IdentityServer4.Models;
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
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly ClientsConfig _clients;

        public Logout(SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IPersistedGrantService persistedGrantService,
            ClientsConfig clients)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _persistedGrantService = persistedGrantService;
            _clients = clients;
        }

        public string SignOutIframeUrl;

        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            if (logoutId == null)
            {
                logoutId = await _interaction.CreateLogoutContextAsync();
            }
            LogoutRequest logout =  await _interaction.GetLogoutContextAsync(logoutId);

            await _signInManager.SignOutAsync();
            
            SignOutIframeUrl = logout?.SignOutIFrameUrl;

            if (logout?.PostLogoutRedirectUri != null)
            {
                return Redirect(logout.PostLogoutRedirectUri);
            }

            return Page();
        }
    }
}