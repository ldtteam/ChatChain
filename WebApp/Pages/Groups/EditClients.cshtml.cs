using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using WebApp.Utilities;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class EditClientsModel : PageModel
    {
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;
        private readonly OrganisationService _organisationsContext;

        public EditClientsModel(GroupService groupsContext, ClientService clientsContext, OrganisationService organisationsContext)
        {
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
            _organisationsContext = organisationsContext;
        }

        public Group Group { get; set; }
        [BindProperty]
        public string[] SelectedClients { get; set; }
        public SelectList ClientOptions { get; set; }
        
        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGetAsync(string organisation, string group)
        {
            if (group == null)
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditGroups);
            Organisation = org;
            if (!result) return NotFound();

            Group = await _groupsContext.GetAsync(new ObjectId(group));

            if (Group == null || Group.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }

            ClientOptions = new SelectList(await _clientsContext.GetFromOwnerIdAsync(Group.OwnerId), nameof(Client.Id), nameof(Client.ClientName));

            SelectedClients = (from client in await _groupsContext.GetClientsAsync(Group.Id) select client.Id.ToString()).ToArray();

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string organisation, string group)
        {
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditGroups);
            Organisation = org;
            if (!result) return NotFound();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Group = await _groupsContext.GetAsync(new ObjectId(group));

            if (Group == null || Group.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }
            
            List<ObjectId> selectedClientsIds = SelectedClients.Select(client => new ObjectId(client)).ToList();

            List<ObjectId> currentClients = Group.ClientIds;

            foreach (ObjectId clientId in currentClients)
            {
                if (!selectedClientsIds.Contains(clientId))
                {
                    await _groupsContext.RemoveClientAsync(Group.Id, clientId);
                }
            }

            foreach (ObjectId selectedClientId in selectedClientsIds)
            {
                if (!currentClients.Contains(selectedClientId))
                {

                    await _groupsContext.AddClientAsync(Group.Id, selectedClientId);
                }
            }
            
            return RedirectToPage("./Clients", new { organisation = Organisation.Id, group = Group.Id} );

        }
    }
}