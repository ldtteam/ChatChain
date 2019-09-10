using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Services;

namespace WebApp.Pages.Clients
{
    public class PreviewNew : PageModel
    {
        private readonly ApiService _apiService;

        public PreviewNew(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Organisation Organisation;

        public Client ApiClient { get; set; }

        [TempData] public string Password { get; set; }

        public async Task<ActionResult> OnGetAsync(Guid organisation, Guid clientId)
        {
            if (Password == null)
                return RedirectToPage("./Index");

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                ApiClient = await client.GetClientAsync(organisation, clientId);
                Organisation = await client.GetOrganisationAsync(organisation);
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }
    }
}