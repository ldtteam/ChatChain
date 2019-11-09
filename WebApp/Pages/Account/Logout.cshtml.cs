using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account
{
    public class LogoutModel : PageModel
    {
        // ReSharper disable once UnusedMember.Global
        public IActionResult OnGet()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}