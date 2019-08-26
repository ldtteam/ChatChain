using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;

namespace IdentityServer.Pages.Account.Manage
{
    [Authorize]
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly EmailSender _emailSender;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailSender emailSender = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            
            [Required]
            public string DisplayName { get; set;}
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string displayName = user.DisplayName;
            string email = await _userManager.GetEmailAsync(user);

            Input = new InputModel
            {
                Email = email,
                DisplayName = displayName
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string email = await _userManager.GetEmailAsync(user);
            string userName = await _userManager.GetUserNameAsync(user);
            if (Input.Email != email || Input.Email != userName)
            {
                if (_userManager.Users.Select(usr => usr.UserName).Contains(Input.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email already in use by another user!");
                    return Page();
                }
                
                IdentityResult setUsernameResult = await _userManager.SetUserNameAsync(user, Input.Email);
                if (!setUsernameResult.Succeeded)
                {
                    foreach (IdentityError error in setUsernameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description.Replace("User Name", "Email"));
                    }
                    return Page();
                }
                
                IdentityResult setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (IdentityError error in setUsernameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
            }

            string displayName = user.DisplayName;
            if (Input.DisplayName != displayName)
            {
                user.DisplayName = Input.DisplayName;
                IdentityResult setDisplayNameResult = await _userManager.UpdateAsync(user);
                if (!setDisplayNameResult.Succeeded)
                {
                    foreach (IdentityError error in setDisplayNameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_emailSender == null)
            {
                return Page();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            string userId = await _userManager.GetUserIdAsync(user);
            string email = await _userManager.GetEmailAsync(user);
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                null,
                new {userId, code },
                Request.Scheme);

            MimeMessage message = new MimeMessage();
            message.To.Add(new MailboxAddress(user.DisplayName, email));
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

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
