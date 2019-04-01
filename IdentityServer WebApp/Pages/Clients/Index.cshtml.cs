using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Store;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClientStore _is4ClientStore;
        public readonly ClientService ClientsContext;
        
        public IndexModel(UserManager<ApplicationUser> userManager, CustomClientStore is4ClientStore, ClientService clientsContext)
        {
            _userManager = userManager;
            _is4ClientStore = is4ClientStore;
            ClientsContext = clientsContext;
        }

        public IList<Client> Clients { get; set; }

        public void OnGet()
        {
            Clients = new List<Client>();

            foreach (var client in ClientsContext.Get())
            {
                if (client != null && client.OwnerId == _userManager.GetUserAsync(User).Result.Id)
                {
                    Clients.Add(client);
                }
            }
        }
    }
}