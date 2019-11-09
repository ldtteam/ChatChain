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
using WebApp.Extensions;
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

        public Organisation Organisation { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<IActionResult> OnGetAsync(Guid organisation, Guid client)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                GetClientResponse response = await apiClient.GetClientAsync(organisation, client);
                Organisation = response.Organisation;
                Client = response.Client;
                if (!Organisation.UserHasPermission(response.User, Permissions.EditClients))
                    return StatusCode(403);

                GetClientConfigResponse getClientConfigResponse =
                    await apiClient.GetClientConfigAsync(organisation, client);
                GetGroupsResponse getGroupsResponse = await apiClient.GetGroupsAsync(organisation);
                IEnumerable<Group> clientGroups =
                    getGroupsResponse.Groups.Where(group => group.ClientIds.Contains(client));

                GroupOptions = new SelectList(clientGroups, nameof(Group.Id), nameof(Group.Name));
                SelectedClientEventGroups = getClientConfigResponse.ClientConfig.ClientEventGroups.ToArray();
                SelectedUserEventGroups = getClientConfigResponse.ClientConfig.UserEventGroups.ToArray();

                return Page();
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnPostAsync(Guid organisation, Guid client)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, client);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                UpdateClientConfigDTO clientConfig = new UpdateClientConfigDTO
                {
                    ClientEventGroups = SelectedClientEventGroups.ToList(),
                    UserEventGroups = SelectedUserEventGroups.ToList()
                };
                await apiClient.UpdateClientConfigAsync(organisation, client, clientConfig);
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