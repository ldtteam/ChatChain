using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityServer.Pages.Account
{
    public class LoginWith2fa : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginWith2fa> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginWith2fa(SignInManager<ApplicationUser> signInManager, ILogger<LoginWith2fa> logger, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            ApplicationUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return Redirect("./Login");
                //throw new InvalidOperationException("Unable to load two-factor authentication user.");
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            ApplicationUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Redirect("./Login");
                //throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            string authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            SignInResult result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);

                return Redirect(returnUrl);
            }
            
            _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return Page();
        }
    }
}