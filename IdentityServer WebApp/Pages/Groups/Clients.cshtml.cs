using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Store;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class ClientsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClientStore _clientStore;
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;
        
        public ClientsModel(UserManager<ApplicationUser> userManager, CustomClientStore clientStore, GroupService groupsContext, ClientService clientsContext)
        {
            _userManager = userManager;
            _clientStore = clientStore;
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
        }

        public IList<IdentityServer4.Models.Client> Clients { get; set; }
        public Group Group { get; set; }
        [BindProperty]
        public string ClientId { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Group = _groupsContext.Get(id);

            if (Group == null || Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            Clients = new List<IdentityServer4.Models.Client>();

            List<string> clientIds = new List<string>();
         
            foreach (var client in _groupsContext.GetClients(Group.Id.ToString()))
            {
                clientIds.Add(client.ClientId);
            }

            foreach (var clientId in clientIds)
            {
                var isClient = await _clientStore.FindClientByIdAsync(clientId);
                var client = _clientsContext.GetFromClientId(clientId);
                
                if (isClient != null && client != null && client.OwnerId == _userManager.GetUserAsync(User).Result.Id)
                {
                    Clients.Add(isClient);
                }    
            }

            return Page();
        }
        
        public IActionResult OnPost(string id)
        {
            
            Console.WriteLine($"Group Id: {id}");
            Console.WriteLine($"Client Id: {ClientId}");
            
            Group = _groupsContext.Get(id);

            var groupClient = _clientsContext.GetFromClientId(ClientId);
            
            if (Group.OwnerId != _userManager.GetUserAsync(User).Result.Id || groupClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
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