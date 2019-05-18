using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    [Authorize]
    public class AboutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}