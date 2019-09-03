using System;
using System.Linq;
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
    public class Index : PageModel
    {
        private readonly OrganisationService _organisationsContext;

        public Index(OrganisationService organisationsContext)
        {
            _organisationsContext = organisationsContext;
        }
        
        public Organisation Organisation { get; private set; }
        
        public OrganisationInvite OrganisationInvite { get; private set; }
        
        public async Task<IActionResult> OnGetAsync(string organisation, string invite)
        {
            (bool result, Organisation org) = await this.VerifyOrganisationId(organisation, _organisationsContext);
            Organisation = org;
            if (!result) return NotFound();

            if (invite == null)
                return StatusCode(403);

            OrganisationInvite = await _organisationsContext.GetInviteAsync(Organisation.Id, invite);

            if (OrganisationInvite == null)
                return StatusCode(403);
            
            if (!string.Equals(OrganisationInvite.Email, User.Claims.First(claim => claim.Type.Equals("EmailAddress")).Value, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string organisation, string invite)
        {
            (bool result, Organisation org) = await this.VerifyOrganisationId(organisation, _organisationsContext);
            Organisation = org;
            if (!result) return NotFound();

            if (invite == null)
                return StatusCode(403);

            OrganisationInvite = await _organisationsContext.GetInviteAsync(Organisation.Id, invite);

            if (OrganisationInvite == null)
                return StatusCode(403);
            
            if (!string.Equals(OrganisationInvite.Email, User.Claims.First(claim => claim.Type.Equals("EmailAddress")).Value, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            Organisation.Users.Add(User.Claims.First(claim => claim.Type.Equals("sub")).Value, new OrganisationUser());

            await _organisationsContext.UpdateAsync(Organisation.Id, Organisation);
            await _organisationsContext.RemoveInviteAsync(Organisation.Id, invite);

            return RedirectToPage("../Index");
        }
    }
}