using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Store;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Client = IdentityServer4.Models.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClientStore _clientStore;
        public readonly ClientService ClientsContext;
        
        public IndexModel(UserManager<ApplicationUser> userManager, CustomClientStore clientStore, ClientService clientsContext)
        {
            _userManager = userManager;
            _clientStore = clientStore;
            ClientsContext = clientsContext;
        }

        public IList<Client> Clients { get; set; }

        public async Task OnGetAsync()
        {
            Clients = new List<Client>();

            foreach (var client in await _clientStore.AllClients())
            {
                var groupClient = ClientsContext.GetFromClientId(client.ClientId);

                if (groupClient != null && groupClient.OwnerId == _userManager.GetUserAsync(User).Result.Id)
                {
                    Clients.Add(client);
                }
            }
        }
    }
}