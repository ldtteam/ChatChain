using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account
{
    [Authorize]
    public class Login : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("/index");
        }
    }
}