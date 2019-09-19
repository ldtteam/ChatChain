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
using WebApp.Extensions;
using WebApp.Services;

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

        public Organisation Organisation { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<IActionResult> OnGetAsync(Guid organisation, Guid group)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                GetGroupResponse getGroupResponse = await apiClient.GetGroupAsync(organisation, group);
                Organisation = getGroupResponse.Organisation;
                Group = getGroupResponse.Group;
                if (!Organisation.UserHasPermission(getGroupResponse.User, Permissions.EditClients))
                    return StatusCode(403);

                GetClientsResponse clientsResponse = await apiClient.GetClientsAsync(organisation);
                ClientOptions = new SelectList(clientsResponse.Clients, nameof(Client.Id),
                    nameof(Client.Name));
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            SelectedClients = Group.ClientIds.ToArray();

            return Page();
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnPostAsync(Guid organisation, Guid group)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, group);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            UpdateGroupDTO updateGroupDTO = new UpdateGroupDTO
            {
                ClientIds = SelectedClients
            };

            try
            {
                await apiClient.UpdateGroupAsync(organisation, group, updateGroupDTO);
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