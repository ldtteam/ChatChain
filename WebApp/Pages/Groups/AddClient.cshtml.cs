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

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class AddClientModel : PageModel
    {
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;

        public AddClientModel(GroupService groupsContext, ClientService clientsContext)
        {
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
        }

        public Group Group { get; set; }
        [BindProperty]
        public string[] SelectedClients { get; set; }
        public SelectList ClientOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("./Clients");
            }

            Group = await _groupsContext.GetAsync(new ObjectId(id));

            if (Group == null || Group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Clients");
            }

            ClientOptions = new SelectList(await _clientsContext.GetFromOwnerIdAsync(Group.OwnerId), nameof(Client.Id), nameof(Client.ClientName));

            SelectedClients = (from client in await _groupsContext.GetClientsAsync(Group.Id) select client.Id.ToString()).ToArray();

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Group = await _groupsContext.GetAsync(new ObjectId(id));

            if (Group == null) return RedirectToPage("./Index");
            
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
            
            return RedirectToPage("./Clients", new { id = Group.Id} );

        }
    }
}