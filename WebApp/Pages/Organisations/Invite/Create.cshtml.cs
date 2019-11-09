using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Extensions;
using WebApp.Services;

namespace WebApp.Pages.Organisations.Invite
{
    [Authorize]
    public class Create : PageModel
    {
        private readonly ApiService _apiService;

        public Create(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty] public InputModel Input { get; set; }

        [TempData]
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Token { get; set; }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class InputModel
        {
            [Required]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "User Email")]
            public string UserEmail { get; set; }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public OrganisationDetails Organisation { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<IActionResult> OnGetAsync(Guid organisation)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                GetOrganisationResponse response = await client.GetOrganisationAsync(organisation);
                Organisation = response.Organisation;
                if (!Organisation.UserHasPermission(response.User, Permissions.CreateOrgUsers))
                    return StatusCode(403);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnPostAsync(Guid organisation)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                CreateInviteDTO inviteDTO = new CreateInviteDTO
                {
                    EmailAddress = Input.UserEmail
                };
                CreateInviteResponse response = await client.CreateInviteAsync(organisation, inviteDTO);
                Token = response.Invite.Token;
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                if (e.StatusCode != 403) return StatusCode(e.StatusCode, e.Response);

                ModelState.AddModelError(string.Empty, e.Response);
                return await OnGetAsync(organisation);
            }

            return RedirectToPage("./Show");
        }
    }
}