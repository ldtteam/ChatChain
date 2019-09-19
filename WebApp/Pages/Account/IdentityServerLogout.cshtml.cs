using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account
{
    public class IdentityServerLogout : PageModel
    {
        // ReSharper disable once UnusedMember.Global
        public IActionResult OnGet()
        {
            HttpContext.SignOutAsync();
            return Page();
        }
    }
}