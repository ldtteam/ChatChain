using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using WebApp.Utilities;

namespace WebApp.Pages.Organisations
{
    public class Leave : PageModel
    {
        private readonly OrganisationService _organisationsContext;
        
        public Leave(OrganisationService organisationsContext)
        {
            _organisationsContext = organisationsContext;
        }
        
        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGet(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyIsMember(organisation, _organisationsContext);
            Organisation = org;
            if (!result) return NotFound();

            // Verify user is not the org owner
            if (Organisation.Owner == User.Claims.First(claim => claim.Type.Equals("sub")).Value)
                return StatusCode(403);

            return Page();
        }
        
        public async Task<IActionResult> OnPost(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyIsMember(organisation, _organisationsContext);
            Organisation = org;
            if (!result) return NotFound();

            // Verify user is not the org owner
            if (Organisation.Owner == User.Claims.First(claim => claim.Type.Equals("sub")).Value)
                return StatusCode(403);

            Organisation.Users.Remove(User.Claims.First(claim => claim.Type.Equals("sub")).Value);
            await _organisationsContext.UpdateAsync(Organisation.Id, Organisation);

            return RedirectToPage("./Index");
        }
    }
}