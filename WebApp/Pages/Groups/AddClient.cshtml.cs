using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityServer.Store;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer_WebApp.Pages.Groups
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
        public IEnumerable<SelectListItem> Clients { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            [Display(Name = "Client")]
            public string ClientId { get; set; }
        }
        
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

            foreach (var client in _clientsContext.GetFromOwnerId(_userManager.GetUserAsync(User).Result.Id))
            {

                if (!clientIds.Contains(client.Id.ToString()))
                {
                    var is4Client = await _clientStore.FindClientByIdAsync(client.ClientId);

                    if (is4Client != null)
                    {
                        clients.Add(new SelectListItem
                        {
                            Text = is4Client.ClientName,
                            Value = is4Client.ClientId
                        });
                    }
                }
            }

            Clients = clients;
            
            return Page();
        }
        
        public IActionResult OnPost(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Group = _groupsContext.Get(id);
            var client = _clientsContext.GetFromClientId(Input.ClientId);

            if (Group != null)
            {
                _groupsContext.AddClient(Group.Id, client.Id);
                return RedirectToPage("./Clients", new { id = Group.Id} );
            }

            return RedirectToPage("./Index");
        }
    }
}