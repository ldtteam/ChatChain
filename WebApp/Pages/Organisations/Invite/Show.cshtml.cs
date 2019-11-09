using System;
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
    public class Show : PageModel
    {
        private readonly ApiService _apiService;

        public Show(ApiService apiService)
        {
            _apiService = apiService;
        }

        [TempData]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Token { get; set; }

        public OrganisationDetails Organisation { get; private set; }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnGetAsync(Guid organisation)
        {
            if (Token == null) return RedirectToPage("../Users/Index");

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
    }
}