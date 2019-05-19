using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IdentityServer.Store;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly CustomClientStore _is4ClientStore;
        private readonly ClientService _clientsContext;

        public EditModel(CustomClientStore is4ClientStore, ClientService clientsContext)
        {
            _is4ClientStore = is4ClientStore;
            _clientsContext = clientsContext;
        }
        
        public Client Client { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Client Name")]
            public string ClientName { get; set; }
            
            [Required]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Client Description")]
            public string ClientDescription { get; set; }
        }
        
        public IActionResult OnGet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Client = _clientsContext.Get(id);
           
            if (Client == null || Client.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }
            
            Input = new InputModel
            {
                ClientName = Client.ClientName,
                ClientDescription = Client.ClientDescription
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
            
            var groupsClient = _clientsContext.Get(id);

            if (groupsClient.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }

            var clientToUpdate = await _is4ClientStore.FindClientByIdAsync(id);

            clientToUpdate.ClientName = Input.ClientName;
            _is4ClientStore.UpdateClient(clientToUpdate);
            
            groupsClient.ClientName = Input.ClientName;
            groupsClient.ClientDescription = Input.ClientDescription;
            _clientsContext.Update(groupsClient.Id, groupsClient);

            return RedirectToPage("./Index");
        } 
    }
}