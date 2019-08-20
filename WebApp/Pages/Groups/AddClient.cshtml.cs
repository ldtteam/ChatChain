using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Store;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class AddClientModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClientStore _clientStore;
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;

        public AddClientModel(UserManager<ApplicationUser> userManager, CustomClientStore clientStore, GroupService groupsContext, ClientService clientsContext)
        {
            _userManager = userManager;
            _clientStore = clientStore;
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
        }

        public Group Group { get; set; }
        [BindProperty]
        public String[] SelectedClients { get; set; }
        public SelectList ClientOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("./Clients");
            }

            Group = _groupsContext.Get(id);

            if (Group == null || Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Clients");
            }
            
            var clients = new List<SelectListItem>();

            var clientIds = new List<string>();

            foreach (var client in _groupsContext.GetClients(Group.Id.ToString()))
            {
                clientIds.Add(client.Id.ToString());
            }
            
            ClientOptions = new SelectList(_clientsContext.GetFromOwnerId(Group.OwnerId), nameof(Client.Id), nameof(Client.ClientName));

            var selectedClients = new List<string>();
            foreach (var client in _groupsContext.GetClients(Group.Id.ToString()))
            {
                selectedClients.Add(client.Id.ToString());
            }
            SelectedClients = selectedClients.ToArray();

            return Page();
        }
        
        public IActionResult OnPost(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Group = _groupsContext.Get(id);

            if (Group == null) return RedirectToPage("./Index");
            
            var selectedClientsIds = SelectedClients.Select(client => new ObjectId(client)).ToList();
            
            foreach (var selectedClientId in selectedClientsIds)
            {
                _groupsContext.AddClient(Group.Id, selectedClientId);
            }

            return RedirectToPage("./Clients", new { id = Group.Id} );

        }
    }
}