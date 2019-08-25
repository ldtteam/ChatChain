using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ClientService _clientsContext;
        
        public IndexModel(ClientService clientsContext)
        {
            _clientsContext = clientsContext;
        }

        public IList<Client> Clients { get; private set; }

        public async Task OnGetAsync()
        {
            Clients = new List<Client>();

            foreach (Client client in await _clientsContext.GetAsync())
            {
                if (client != null && client.OwnerId == User.Claims.First(claim => claim.Type.Equals("sub")).Value)
                {
                    Clients.Add(client);
                }
            }
        }
    }
}