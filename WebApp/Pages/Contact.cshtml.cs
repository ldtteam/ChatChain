using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer_WebApp.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Your contact page.";
        }
    }
}