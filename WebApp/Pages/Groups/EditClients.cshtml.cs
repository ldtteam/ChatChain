using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Api;
using WebApp.Services;
using Client = WebApp.Api.Client;
using Group = WebApp.Api.Group;
using Organisation = WebApp.Api.Organisation;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class EditClientsModel : PageModel
    {
        private readonly ApiService _apiService;

        public EditClientsModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Group Group { get; set; }
        [BindProperty] public Guid[] SelectedClients { get; set; }
        public SelectList ClientOptions { get; set; }

        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid organisation, Guid group)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                await apiClient.CanUpdateGroupAsync(false, organisation, group);
                Organisation = await apiClient.GetOrganisationAsync(organisation);
                Group = await apiClient.GetGroupAsync(organisation, group);
                ClientOptions = new SelectList(await apiClient.GetClientsAsync(organisation), nameof(Client.Id),
                    nameof(Client.ClientName));
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            SelectedClients = Group.ClientIds.ToArray();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid organisation, Guid group)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, group);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                Group = await apiClient.GetGroupAsync(organisation, group);
                Group.ClientIds = SelectedClients;
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            try
            {
                await apiClient.UpdateGroupAsync(organisation, group, Group);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                if (e.StatusCode != 403) return StatusCode(e.StatusCode, e.Response);

                ModelState.AddModelError(string.Empty, e.Response);
                return await OnGetAsync(organisation, group);
            }

            return RedirectToPage("./Index");
        }
    }
}