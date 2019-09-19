using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Services;

namespace WebApp.Pages.Organisations.Invite
{
    [Authorize]
    public class Index : PageModel
    {
        private readonly ApiService _apiService;

        public Index(ApiService apiService)
        {
            _apiService = apiService;
        }

        public OrganisationDetails Organisation { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<IActionResult> OnGetAsync(Guid organisation, string invite)
        {
            if (invite == null)
                return StatusCode(403);
            
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                GetOrganisationResponse response = await client.GetOrganisationAsync(organisation);
                Organisation = response.Organisation;
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnPostAsync(Guid organisation, string invite)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, invite);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                await client.UseInviteAsync(organisation, invite);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                if (e.StatusCode != 403) return StatusCode(e.StatusCode, e.Response);

                ModelState.AddModelError(string.Empty, e.Response);
                return await OnGetAsync(organisation, invite);
            }

            return RedirectToPage("../Index");
        }
    }
}