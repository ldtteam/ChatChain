using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account
{
    public class IdentityServerLogout : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.SignOutAsync();
            return Page();
        }
    }
}