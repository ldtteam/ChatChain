using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Extensions;
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

        public Client Client { get; set; }

        [TempData] public string Password { get; set; }

        // ReSharper disable once UnusedMember.Global
        public async Task<ActionResult> OnGetAsync(Guid organisation, Guid client)
        {
            if (Password == null)
                return RedirectToPage("./Index");

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                GetClientResponse response = await apiClient.GetClientAsync(organisation, client);
                Organisation = response.Organisation;
                Client = response.Client;
                if (!Organisation.UserHasPermission(response.User, Permissions.CreateClients))
                    return StatusCode(403);
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }
    }
}