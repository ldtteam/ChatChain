using System;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Utilities;

namespace WebApp.Pages.Organisations.Invite
{
    [Authorize]
    public class Show : PageModel
    {
        private readonly OrganisationService _organisationsContext;

        public Show(OrganisationService organisationsContext)
        {
            _organisationsContext = organisationsContext;
        }
        
        [TempData]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Token { get; set; }
        
        public Organisation Organisation { get; private set; }

        public async Task<IActionResult> OnGetAsync(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.CreateOrgUsers);
            Organisation = org;
            if (!result) return NotFound();
            
            if (Token == null)
                return RedirectToPage("../Index");
            
            return Page();
        }
    }
}