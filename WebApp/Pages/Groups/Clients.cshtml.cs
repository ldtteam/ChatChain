using System.Collections.Generic;
using System.Linq;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class ClientsModel : PageModel
    {
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;
        
        public ClientsModel(GroupService groupsContext, ClientService clientsContext)
        {
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
        }

        public IList<Client> Clients { get; private set; }
        public Group Group { get; private set; }
        [BindProperty]
        public string ClientId { get; set; }

        public IActionResult OnGet(string id)
        {
            Group = _groupsContext.Get(id);

            if (Group == null || Group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }
            
            Clients = new List<Client>();

            foreach (Client client in _groupsContext.GetClients(Group.Id.ToString()))
            {
                if (client.OwnerId == User.Claims.First(claim => claim.Type.Equals("sub")).Value)
                {
                    Clients.Add(client);
                }
            }

            return Page();
        }
        
        public IActionResult OnPost(string id)
        {
            Group = _groupsContext.Get(id);

            Client groupClient = _clientsContext.Get(ClientId);
            
            if (Group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value || groupClient.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }
            
            try
            {
                _groupsContext.RemoveClient(Group.Id, groupClient.Id);
                
                return RedirectToPage("./Clients", new { id = Group.Id});
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("./Clients",
                    new { id = Group.Id , saveChangesError = true });
            }
        }
    }
}