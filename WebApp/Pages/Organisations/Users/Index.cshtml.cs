using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Services;

namespace WebApp.Pages.Organisations.Users
{
    [Authorize]
    public class Index : PageModel
    {
        private readonly ApiService _apiService;

        public Index(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IEnumerable<OrganisationUser> Users { get; private set; }

        public Organisation Organisation { get; private set; }

        public OrganisationUser OrganisationUser { get; private set; }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnGetAsync(Guid organisation)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                GetOrganisationUsersResponse response = await apiClient.GetUsersAsync(organisation);
                Organisation = response.Organisation;
                OrganisationUser = response.User;
                Users = response.RequestedUsers;
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