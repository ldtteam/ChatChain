using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace IdentityServer.Pages.Account
{
    [AllowAnonymous]
    public class Register : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<Register> _logger;
        private readonly EmailSender _emailSender;

        public Register(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<Register> logger,
            EmailSender emailSender = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid) return Page();
            
            ApplicationUser user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, DisplayName = Input.Username};
            IdentityResult result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {Input.Username} created a new account.");

                if (_emailSender != null)
                {
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        null,
                        new { userId = user.Id, code },
                        Request.Scheme);

                    MimeMessage message = new MimeMessage();
                    message.To.Add(new MailboxAddress(user.DisplayName, Input.Email));
                    message.Subject = "Confirm ChatChain Auth Account";
            
                    TextPart plainBody = new TextPart("plain")
                    {
                        Text = $"Please confirm your account at: {callbackUrl}"
                    };
                    TextPart htmlBody = new TextPart("html")
                    {
                        Text = $"Please confirm your account at: <html><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a></html>"
                    };

                    MultipartAlternative alternative = new MultipartAlternative {plainBody, htmlBody};

                    message.Body = alternative;

                    await _emailSender.SendEmailAsync(message);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
