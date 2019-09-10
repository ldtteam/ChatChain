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
using Group = WebApp.Api.Group;
using Organisation = WebApp.Api.Organisation;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApiService _apiService;

        public EditModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public Group Group { get; set; }
        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public Organisation Organisation { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Group Name")]
            public string GroupName { get; set; }

            [Required]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Group Description")]
            public string GroupDescription { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid organisation, Guid group)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                await client.CanUpdateGroupAsync(false, organisation, group);
                Organisation = await client.GetOrganisationAsync(organisation);
                Group = await client.GetGroupAsync(organisation, group);
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid organisation, Guid group)
        {
            if (!ModelState.IsValid) return await OnGetAsync(organisation, group);

            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient client = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                Group = await client.GetGroupAsync(organisation, group);
                Group.GroupName = Input.GroupName;
                Group.GroupDescription = Input.GroupDescription;
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            try
            {
                await client.UpdateGroupAsync(organisation, group, Group);
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