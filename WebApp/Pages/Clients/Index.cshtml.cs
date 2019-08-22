using System.Collections.Generic;
using System.Linq;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CustomClientStore _is4ClientStore;
        private readonly ClientService _clientsContext;
        
        public IndexModel(CustomClientStore is4ClientStore, ClientService clientsContext)
        {
            _is4ClientStore = is4ClientStore;
            _clientsContext = clientsContext;
        }

        public IList<Client> Clients { get; set; }

        public void OnGet()
        {
            Clients = new List<Client>();

            foreach (Client client in _clientsContext.Get())
            {
                if (client != null && client.OwnerId == User.Claims.First(claim => claim.Type.Equals("sub")).Value)
                {
                    Clients.Add(client);
                }
            }
        }
    }
}