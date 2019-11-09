using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Extensions;
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

        public OrganisationUser RemoveUser { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<IActionResult> OnGetAsync(Guid organisation, string id)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                GetOrganisationUserResponse response = await client.GetUserAsync(organisation, id);
                RemoveUser = response.User;
                if (!response.Organisation.UserHasPermission(response.User, Api.Permissions.DeleteOrgUsers) &&
                    response.Organisation.UserIsOwner(RemoveUser) ||
                    id == response.User.Id)
                    return StatusCode(403);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnPostAsync(Guid organisation, string id)
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