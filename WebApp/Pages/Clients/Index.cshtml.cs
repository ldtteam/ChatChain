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

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;

        public IndexModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Organisation Organisation { get; set; }

        public ICollection<Client> Clients { get; private set; }

        public ApiClient ApiClient { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid organisation)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                Organisation = await ApiClient.GetOrganisationAsync(organisation);
                Clients = await ApiClient.GetClientsAsync(organisation);
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }
    }
}