using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Extensions;
using WebApp.Services;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApiService _apiService;

        public EditModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Client Client { get; set; }
        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public Organisation Organisation { get; private set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Client Name")]
            public string ClientName { get; set; }

            [Required]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Client Description")]
            public string ClientDescription { get; set; }
        }

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
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnPostAsync(Guid organisation, Guid client)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, client);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            UpdateClientDTO updateClientDTO = new UpdateClientDTO
            {
                Name = Input.ClientName,
                Description = Input.ClientDescription
            };

            try
            {
                await apiClient.UpdateClientAsync(organisation, client, updateClientDTO);
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