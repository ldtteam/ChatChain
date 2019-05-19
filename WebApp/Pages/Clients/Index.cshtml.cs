using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Store;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CustomClientStore _is4ClientStore;
        public readonly ClientService ClientsContext;
        
        public IndexModel(CustomClientStore is4ClientStore, ClientService clientsContext)
        {
            _is4ClientStore = is4ClientStore;
            ClientsContext = clientsContext;
        }

        public IList<Client> Clients { get; set; }

        public void OnGet()
        {
            Clients = new List<Client>();

            foreach (var client in ClientsContext.Get())
            {
                if (client != null && client.OwnerId == User.Claims.First(claim => claim.Type.Equals("sub")).Value)
                {
                    Clients.Add(client);
                }
            }
        }
    }
}