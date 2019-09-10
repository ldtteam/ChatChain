using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Api;
using WebApp.Services;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;

namespace WebApp.Pages.Organisations.Users
{
    [Authorize]
    public class Permissions : PageModel
    {
        private readonly ApiService _apiService;

        public Permissions(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Organisation Organisation { get; set; }

        public ResponseUser EditingUser { get; set; }

        [BindProperty] public ICollection<Api.Permissions> SelectedPermissions { get; set; }

        public SelectList PermissionOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid organisation, string id)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);
            try
            {
                await client.CanUpdateUserAsync(false, organisation, id);
                Organisation = await client.GetOrganisationAsync(organisation);
                EditingUser = await client.GetUserAsync(organisation, id);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                return StatusCode(e.StatusCode, e.Response);
            }

            PermissionOptions = new SelectList(Enum.GetValues(typeof(Api.Permissions)), nameof(Api.Permissions),
                nameof(Api.Permissions));

            SelectedPermissions = EditingUser.OrganisationUser.Permissions;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid organisation, string id)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, id);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);
            EditingUser = await client.GetUserAsync(organisation, id);
            EditingUser.OrganisationUser.Permissions = SelectedPermissions;

            try
            {
                await client.UpdateUserAsync(organisation, id, EditingUser.OrganisationUser);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                if (e.StatusCode != 403) return StatusCode(e.StatusCode, e.Response);

                ModelState.AddModelError(string.Empty, e.Response);
                return await OnGetAsync(organisation, id);
            }

            return RedirectToPage("./Index");
        }
    }
}