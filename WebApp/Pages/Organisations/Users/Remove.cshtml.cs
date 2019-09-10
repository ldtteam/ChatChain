using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Services;

namespace WebApp.Pages.Organisations.Users
{
    [Authorize]
    public class Remove : PageModel
    {
        private readonly ApiService _apiService;

        public Remove(ApiService apiService)
        {
            _apiService = apiService;
        }

        public ResponseUser RemoveUser { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid organisation, string id)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);
            try
            {
                await client.CanDeleteUserAsync(false, organisation, id);
                RemoveUser = await client.GetUserAsync(organisation, id);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(Guid organisation, string id)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, id);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);
            try
            {
                await client.DeleteUserAsync(organisation, id);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                if (e.StatusCode != 403) return StatusCode(e.StatusCode, e.Response);

                ModelState.AddModelError(string.Empty, e.Response);
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}