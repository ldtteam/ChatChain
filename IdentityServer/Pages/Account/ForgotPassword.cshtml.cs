using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;

namespace IdentityServer.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPassword : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSender _emailSender;

        public ForgotPassword(UserManager<ApplicationUser> userManager, EmailSender emailSender = null)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Page();
                }

                if (_emailSender != null)
                {
                    // For more information on how to enable account confirmation and password reset please 
                    // visit https://go.microsoft.com/fwlink/?LinkID=532713
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new {code},
                        protocol: Request.Scheme);

                    var message = new MimeMessage();
                    message.To.Add(new MailboxAddress(user.Name, Input.Email));
                    message.Subject = "Confirm ChatChain Auth Account";

                    var plainBody = new TextPart("plain")
                    {
                        Text = $"Please confirm your account at: {callbackUrl}"
                    };
                    var htmlBody = new TextPart("html")
                    {
                        Text =
                            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>"
                    };

                    var alternative = new MultipartAlternative();
                    alternative.Add(plainBody);
                    alternative.Add(htmlBody);

                    message.Body = alternative;

                    await _emailSender.SendEmailAsync(message);
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
            }

            return Page();
        }
    }
}
