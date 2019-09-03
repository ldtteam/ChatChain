using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using WebApp.Utilities;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class ClientsModel : PageModel
    {
        private readonly GroupService _groupsContext;
        private readonly OrganisationService _organisationsContext;
        
        public ClientsModel(GroupService groupsContext, OrganisationService organisationsContext)
        {
            _groupsContext = groupsContext;
            _organisationsContext = organisationsContext;
        }

        public IList<Client> Clients { get; private set; }
        public Group Group { get; private set; }
        
        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGetAsync(string organisation, string group)
        {
            (bool result, Organisation org) = await this.VerifyIsMember(organisation, _organisationsContext);
            Organisation = org;
            if (!result) return NotFound();
            
            Group = await _groupsContext.GetAsync(new ObjectId(group));

            if (Group == null || Group.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }
            
            Clients = new List<Client>();

            foreach (Client client in await _groupsContext.GetClientsAsync(Group.Id))
            {
                if (client.OwnerId == Organisation.Id.ToString())
                {
                    Clients.Add(client);
                }
            }

            return Page();
        }
    }
}