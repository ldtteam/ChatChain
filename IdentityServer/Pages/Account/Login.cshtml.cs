using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Pages.Account
{
    public class Login : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<Login> _logger;

        public Login(SignInManager<ApplicationUser> signInManager, ILogger<Login> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }
        
        public string ReturnUrl { get; set; }
        
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid) 
                return Page();
            
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return Redirect(returnUrl);
            }
                
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();

        }
    }
}