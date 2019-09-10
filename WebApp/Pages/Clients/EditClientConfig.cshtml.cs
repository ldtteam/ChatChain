using System;
using System.Collections.Generic;
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

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class EditClientConfig : PageModel
    {
        private readonly ApiService _apiService;

        public EditClientConfig(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Client Client { get; set; }
        [BindProperty] public Guid[] SelectedClientEventGroups { get; set; }
        [BindProperty] public Guid[] SelectedUserEventGroups { get; set; }
        public SelectList GroupOptions { get; set; }

        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid organisation, Guid client)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                await apiClient.CanUpdateClientConfigAsync(false, organisation, client);
                Organisation = await apiClient.GetOrganisationAsync(organisation);
                Client = await apiClient.GetClientAsync(organisation, client);
                ClientConfig clientConfig = await apiClient.GetClientConfigAsync(organisation, Client.Id);
                ICollection<Group> clientGroups = await apiClient.GetGroupsForClientAsync(organisation, Client.Id);

                GroupOptions = new SelectList(clientGroups, nameof(Group.Id), nameof(Group.GroupName));
                SelectedClientEventGroups = clientConfig.ClientEventGroups.ToArray();
                SelectedUserEventGroups = clientConfig.UserEventGroups.ToArray();

                return Page();
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid organisation, Guid client)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, client);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                Client = await apiClient.GetClientAsync(organisation, client);
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            try
            {
                ClientConfig clientConfig = new ClientConfig
                {
                    ClientEventGroups = SelectedClientEventGroups.ToList(),
                    UserEventGroups = SelectedUserEventGroups.ToList()
                };
                await apiClient.UpdateClientConfigAsync(organisation, Client.Id, clientConfig);
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