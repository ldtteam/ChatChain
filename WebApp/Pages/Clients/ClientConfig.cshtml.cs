using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
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

        public ClientConfig(ClientService clientsContext, ClientConfigService clientConfigsContext)
        {
            _clientsContext = clientsContext;
            _clientConfigsContext = clientConfigsContext;
        }

        public Client Client { get; set; }
        [BindProperty]
        public String[] SelectedClientEventGroups { get; set; }
        [BindProperty]
        public String[] SelectedUserEventGroups { get; set; }
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
                var newConfig = new Models.ClientConfig
                {
                    ClientId = Client.Id
                };
                _clientConfigsContext.Create(newConfig);
            }
            GroupOptions = new SelectList(_clientsContext.GetGroups(Client.Id), nameof(Group.Id), nameof(Group.GroupName));
            
            var clientEventGroupIds = new List<string>();

            foreach (var groupId in _clientsContext.GetClientConfig(Client.Id).ClientEventGroups)
            {
                clientEventGroupIds.Add(groupId.ToString());
            }

            var userEventGroupIds = new List<string>();
            
            foreach (var groupId in _clientsContext.GetClientConfig(Client.Id).UserEventGroups)
            {
                userEventGroupIds.Add(groupId.ToString());
            }
            
            if (_clientsContext.GetClientConfig(Client.Id) != null)
            {
                SelectedClientEventGroups = clientEventGroupIds.ToArray();
                SelectedUserEventGroups = userEventGroupIds.ToArray();
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string clientId)
        {
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

            var clientEventGroupIds = SelectedClientEventGroups.Select(group => new ObjectId(group)).ToList();
            var userEventGroupIds = SelectedUserEventGroups.Select(group => new ObjectId(group)).ToList();

            var clientConfig = _clientsContext.GetClientConfig(Client.Id);
            clientConfig.ClientEventGroups = clientEventGroupIds;
            clientConfig.UserEventGroups = userEventGroupIds;
            _clientConfigsContext.Update(clientConfig.Id, clientConfig);
            
            return RedirectToPage("./Index");
        }
    }
}