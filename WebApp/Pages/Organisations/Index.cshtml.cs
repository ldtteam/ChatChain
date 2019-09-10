using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Api;
using WebApp.Services;
using Organisation = WebApp.Api.Organisation;

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

        public IList<Organisation> Organisations { get; private set; }

        public ApiClient Client { get; set; }

        public async Task<ActionResult> OnGet()
        {
            if (!await _apiService.VerifyTokensAsync(HttpContext))
                return SignOut(new AuthenticationProperties {RedirectUri = HttpContext.Request.GetDisplayUrl()},
                    "Cookies");
            Client = await _apiService.GetApiClientAsync(HttpContext);

            ICollection<Organisation> organisations = await Client.GetOrganisationsAsync();
            Organisations = new List<Organisation>();

            foreach (Organisation organisation in organisations) Organisations.Add(organisation);
            return Page();
        }
    }
}