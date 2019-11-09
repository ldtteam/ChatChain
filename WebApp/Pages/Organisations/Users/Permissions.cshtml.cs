using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Extensions;
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

        public Organisation Organisation { get; private set; }

        public OrganisationUser EditingUser { get; set; }

        [BindProperty] public ICollection<Api.Permissions> SelectedPermissions { get; set; }


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
                Organisation = response.Organisation;
                EditingUser = response.RequestedUser;
                if (!Organisation.UserHasPermission(response.User, Api.Permissions.EditOrgUsers) &&
                    Organisation.UserIsOwner(EditingUser) ||
                    id == response.User.Id)
                    return StatusCode(403);
            }
            catch (ApiException e)
            {
                // Relays the status code and response from the API
                return StatusCode(e.StatusCode, e.Response);
            }

            SelectedPermissions = EditingUser.Permissions;

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
                UpdateOrganisationUserDTO updateDTO = new UpdateOrganisationUserDTO
                {
                    //These types are identical, however NSwag/Swagger is a little dumb on the generation, and creates a duplicate.
                    //so we case the normal permission enum to the one that's wanted. This is easier than fixing the apiClient code
                    //because it will persist if the code is regenerated.
                    Permissions = SelectedPermissions.Select(perm => (Permissions2) perm).ToList()
                };
                await client.UpdateUserAsync(organisation, id, updateDTO);
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