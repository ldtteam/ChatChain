using System;
using System.ComponentModel.DataAnnotations;
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
    public class CreateModel : PageModel
    {
        private readonly ApiService _apiService;

        public CreateModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Organisation Organisation { get; set; }

        [BindProperty] public InputModel Input { get; set; }

        [TempData] public string Password { get; set; }


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

        public async Task<IActionResult> OnGetAsync(Guid organisation)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                await client.CanCreateClientAsync(false, organisation);
                Organisation = await client.GetOrganisationAsync(organisation);
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid organisation)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            Client apiClient = new Client
            {
                ClientName = Input.ClientName,
                ClientDescription = Input.ClientDescription
            };

            try
            {
                CreateClientResponse response = await client.CreateClientAsync(organisation, apiClient);
                Password = response.Password;
                return RedirectToPage("./PreviewNew", new {organisation, clientId = response.Id});
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                if (e.StatusCode != 403) return StatusCode(e.StatusCode, e.Response);

                ModelState.AddModelError(string.Empty, e.Response);
                return await OnGetAsync(organisation);
            }
        }
    }
}