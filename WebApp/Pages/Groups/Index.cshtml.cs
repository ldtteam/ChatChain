using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Services;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;

        public IndexModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public ICollection<Group> Groups { get; private set; }

        public Organisation Organisation { get; private set; }

        public OrganisationUser OrganisationUser { get; private set; }

        // ReSharper disable once UnusedMember.Global
        public async Task<IActionResult> OnGetAsync(Guid organisation)
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            try
            {
                GetGroupsResponse response = await apiClient.GetGroupsAsync(organisation);
                Organisation = response.Organisation;
                OrganisationUser = response.User;
                Groups = response.Groups;
            }
            catch (ApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }

            return Page();
        }
    }
}