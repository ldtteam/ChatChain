using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class ClientConfig : PageModel
    {
        private readonly ClientService _clientsContext;
        private readonly ClientConfigService _clientConfigsContext;

        public ClientConfig(UserManager<ApplicationUser> userManager, ClientService clientsContext, ClientConfigService clientConfigsContext)
        {
            _clientsContext = clientsContext;
            _clientConfigsContext = clientConfigsContext;
        }

        public Client Client { get; set; }
        public ObjectId[] SelectedGroups { get; set; }
        public SelectList GroupOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(string clientId)
        {
            if (clientId.IsNullOrEmpty())
            {
                return RedirectToPage("./Index");
            }

            Client = _clientsContext.Get(clientId); 
            
            if (_clientConfigsContext.Get(Client.ClientConfigId) == null)
            {
                Console.WriteLine("Creating Client Config");
                ObjectId configId = new ObjectId();
                var newConfig = new Models.ClientConfig
                {
                    Id = configId,
                    ClientId = Client.Id
                };
                _clientConfigsContext.Create(newConfig);
            }
            
            Client = _clientsContext.Get(clientId); //if the clientConfig was created we need to update this to get the ID for it.
            
            GroupOptions = new SelectList(_clientsContext.GetGroups(Client.Id), nameof(Group.Id), nameof(Group.GroupName));
            
            Console.WriteLine("Client Config: " + _clientsContext.GetClientConfig(Client.Id));
            
            /*Console.WriteLine("Client Config Event Groups: " + _clientsContext.GetClientConfig(Client.Id).clientEventGroups);
            if (_clientsContext.GetClientConfig(Client.Id) != null)
            {
                SelectedGroups = _clientsContext.GetClientConfig(Client.Id).clientEventGroups.ToArray();
            }*/

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string clientId)
        {
            Console.WriteLine("List: " + SelectedGroups);
            
            if (clientId.IsNullOrEmpty())
            {
                return RedirectToPage("./Index");
            }

            Client = _clientsContext.Get(clientId);

            if (_clientConfigsContext.Get(Client.ClientConfigId) == null)
            {
                Console.WriteLine("ClientConfig is null for client: " + clientId);
                return RedirectToPage("./Index");
            }

            var clientConfig = _clientsContext.GetClientConfig(Client.Id);
            clientConfig.clientEventGroups = SelectedGroups.ToList();
            _clientConfigsContext.Update(clientConfig.Id, clientConfig);
            
            return Page();
        }
    }
}