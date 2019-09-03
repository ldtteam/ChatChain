using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Utilities;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly GroupService _groupsContext;
        private readonly OrganisationService _organisationsContext; 
        
        public IndexModel(GroupService groupsContext, OrganisationService organisationsContext)
        {
            _groupsContext = groupsContext;
            _organisationsContext = organisationsContext;
        }

        public IList<Group> Groups { get; private set; }
        
        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGetAsync(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyIsMember(organisation, _organisationsContext);
            Organisation = org;
            if (!result) return NotFound();
            
            Groups = new List<Group>();

            foreach (Group group in await _groupsContext.GetFromOwnerAsync(Organisation.Id.ToString()))
            {
                Groups.Add(group);
            }

            return Page();
        }
    }
}