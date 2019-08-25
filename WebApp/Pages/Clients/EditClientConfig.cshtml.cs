using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class EditClientConfig : PageModel
    {
        private readonly ClientService _clientsContext;
        private readonly ClientConfigService _clientConfigsContext;

        public EditClientConfig(ClientService clientsContext, ClientConfigService clientConfigsContext)
        {
            _clientsContext = clientsContext;
            _clientConfigsContext = clientConfigsContext;
        }

        public Client Client { get; set; }
        [BindProperty]
        public string[] SelectedClientEventGroups { get; set; }
        [BindProperty]
        public string[] SelectedUserEventGroups { get; set; }
        public SelectList GroupOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(string clientId)
        {
            if (clientId.IsNullOrEmpty())
            {
                return RedirectToPage("./Index");
            }

            Client = await _clientsContext.GetAsync(new ObjectId(clientId)); 
            
            if (await _clientConfigsContext.GetAsync(Client.ClientConfigId) == null)
            {
                ClientConfig newConfig = new ClientConfig
                {
                    ClientId = Client.Id
                };
                await _clientConfigsContext.CreateAsync(newConfig);
            }
            GroupOptions = new SelectList(await _clientsContext.GetGroupsAsync(Client.Id), nameof(Group.Id), nameof(Group.GroupName));
            
            List<string> clientEventGroupIds = new List<string>();

            foreach (ObjectId groupId in (await _clientsContext.GetClientConfigAsync(Client.Id)).ClientEventGroups)
            {
                clientEventGroupIds.Add(groupId.ToString());
            }

            List<string> userEventGroupIds = new List<string>();
            
            foreach (ObjectId groupId in (await _clientsContext.GetClientConfigAsync(Client.Id)).UserEventGroups)
            {
                userEventGroupIds.Add(groupId.ToString());
            }

            if (await _clientsContext.GetClientConfigAsync(Client.Id) == null) return Page();
            SelectedClientEventGroups = clientEventGroupIds.ToArray();
            SelectedUserEventGroups = userEventGroupIds.ToArray();

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string clientId)
        {
            if (clientId.IsNullOrEmpty())
            {
                return RedirectToPage("./Index");
            }

            Client = await _clientsContext.GetAsync(new ObjectId(clientId));

            if (await _clientConfigsContext.GetAsync(Client.ClientConfigId) == null)
            {
                Console.WriteLine("ClientConfig is null for client: " + clientId);
                return RedirectToPage("./Index");
            }

            List<ObjectId> clientEventGroupIds = SelectedClientEventGroups.Select(group => new ObjectId(group)).ToList();
            List<ObjectId> userEventGroupIds = SelectedUserEventGroups.Select(group => new ObjectId(group)).ToList();

            ClientConfig clientConfig = await _clientsContext.GetClientConfigAsync(Client.Id);
            clientConfig.ClientEventGroups = clientEventGroupIds;
            clientConfig.UserEventGroups = userEventGroupIds;
            await _clientConfigsContext.UpdateAsync(clientConfig.Id, clientConfig);
            
            return RedirectToPage("./Index");
        }
    }
}