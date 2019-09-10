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

        public IDictionary<string, ResponseUser> Users { get; private set; }

        public ApiClient Client;

        public Organisation Organisation { get; private set; }

        public async Task<IActionResult> OnGet(Guid organisation)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            Client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                Organisation = await Client.GetOrganisationAsync(organisation);
                Users = await Client.GetUsersAsync(organisation);
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