using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityServer.Store;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Client = IdentityServer4.Models.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClientStore _clientStore;
        private readonly ClientService _clientsContext;

        public EditModel(UserManager<ApplicationUser> userManager, CustomClientStore clientStore, ClientService clientsContext)
        {
            _userManager = userManager;
            _clientStore = clientStore;
            _clientsContext = clientsContext;
        }
        
        public Client Client { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            [Display(Name = "Client Name")]
            public string ClientName { get; set; }
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Client = await _clientStore.FindClientByIdAsync(id);
            
            
            var groupsClient = _clientsContext.GetFromClientId(id);

            if (groupsClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            if (Client == null)
            {
                return NotFound();
            }
            
            Input = new InputModel
            {
                ClientName = Client.ClientName
            };

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var groupsClient = _clientsContext.GetFromClientId(id);

            if (groupsClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }

            var clientToUpdate = await _clientStore.FindClientByIdAsync(id);

            clientToUpdate.ClientName = Input.ClientName;
            _clientStore.UpdateClient(clientToUpdate);
            
            groupsClient.ClientName = Input.ClientName;
            _clientsContext.Update(groupsClient.Id.ToString(), groupsClient);

            return RedirectToPage("./Index");
        } 
    }
}