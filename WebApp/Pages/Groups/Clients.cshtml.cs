using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class ClientsModel : PageModel
    {
        private readonly GroupService _groupsContext;
        
        public ClientsModel(GroupService groupsContext)
        {
            _groupsContext = groupsContext;
        }

        public IList<Client> Clients { get; private set; }
        public Group Group { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Group = await _groupsContext.GetAsync(new ObjectId(id));

            if (Group == null || Group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }
            
            Clients = new List<Client>();

            foreach (Client client in await _groupsContext.GetClientsAsync(Group.Id))
            {
                if (client.OwnerId == User.Claims.First(claim => claim.Type.Equals("sub")).Value)
                {
                    Clients.Add(client);
                }
            }

            return Page();
        }
    }
}