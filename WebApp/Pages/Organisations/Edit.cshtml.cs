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

namespace WebApp.Pages.Organisations
{
    [Authorize]
    public class Edit : PageModel
    {
        private readonly ApiService _apiService;

        public Edit(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty] public InputModel Input { get; set; }

        public Organisation Organisation { get; set; }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Organisation Name")]
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid organisation)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                await client.CanUpdateOrganisationAsync(false, organisation);
                Organisation = await client.GetOrganisationAsync(organisation);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
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

            try
            {
                Organisation = await client.GetOrganisationAsync(organisation);
            }
            catch (ApiException e2)
            {
                return StatusCode(e2.StatusCode, e2.Response);
            }

            if (Organisation.Name == Input.Name)
            {
                ModelState.AddModelError(string.Empty, "No Changes Made");
                return Page();
            }

            Organisation.Name = Input.Name;

            try
            {
                await client.DeleteOrganisationAsync(organisation);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                if (!(e.StatusCode == 404 || e.StatusCode == 403)) return StatusCode(e.StatusCode, e.Response);
                ModelState.AddModelError(string.Empty, e.Response);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}