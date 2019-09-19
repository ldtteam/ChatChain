using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Services;

// ReSharper disable UnusedMember.Global

namespace WebApp.Pages.Organisations
{
    [Authorize]
    public class Index : PageModel
    {
        private readonly ApiService _apiService;

        public Index(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IEnumerable<OrganisationDetails> Organisations { get; private set; }
        public IDictionary<string, OrganisationUser> OrganisationUsers { get; private set; }

        public async Task<ActionResult> OnGetAsync()
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            ApiClient apiClient = await _apiService.GetApiClientAsync(HttpContext);

            GetOrganisationsResponse response = await apiClient.GetOrganisationsAsync();
            Organisations = response.Organisations;
            OrganisationUsers = response.Users;

            return Page();
        }
    }
}