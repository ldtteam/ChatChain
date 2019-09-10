using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Services;
using Client = WebApp.Api.Client;
using Organisation = WebApp.Api.Organisation;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApiService _apiService;

        public DeleteModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty] public Client Client { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string ErrorMessage { get; set; }

        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid organisation, Guid client)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                await apiClient.CanDeleteClientAsync(false, organisation, client);
                Organisation = await apiClient.GetOrganisationAsync(organisation);
                Client = await apiClient.GetClientAsync(organisation, client);
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid organisation, Guid client)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                await apiClient.DeleteClientAsync(organisation, client);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                if (e.StatusCode != 403) return StatusCode(e.StatusCode, e.Response);

                ModelState.AddModelError(string.Empty, e.Response);
                return await OnGetAsync(organisation, client);
            }

            return RedirectToPage("./Index");
        }
    }
}