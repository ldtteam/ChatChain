using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using WebApp.Utilities;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly CustomClientStore _is4ClientStore;
        private readonly ClientService _clientsContext;
        private readonly OrganisationService _organisationsContext;

        public EditModel(CustomClientStore is4ClientStore, ClientService clientsContext, OrganisationService organisationsContext)
        {
            _is4ClientStore = is4ClientStore;
            _clientsContext = clientsContext;
            _organisationsContext = organisationsContext;
        }
        
        public Client Client { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public Organisation Organisation { get; set; }

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
        
        public async Task<IActionResult> OnGetAsync(string organisation, string client)
        {
            if (client == null)
            {
                return NotFound();
            }
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditClients);
            Organisation = org;
            if (!result) return NotFound();

            Client = await _clientsContext.GetAsync(new ObjectId(client));
           
            if (Client == null || Client.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }
            
            Input = new InputModel
            {
                ClientName = Client.ClientName,
                ClientDescription = Client.ClientDescription
            };

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string organisation, string client)
        {
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditClients);
            Organisation = org;
            if (!result) return NotFound();
            
            if (client == null)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Client groupsClient = await _clientsContext.GetAsync(new ObjectId(client));

            if (groupsClient.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }

            IdentityServer4.Models.Client clientToUpdate = await _is4ClientStore.FindClientByIdAsync(groupsClient.ClientId);

            clientToUpdate.ClientName = Input.ClientName;
            _is4ClientStore.UpdateClient(clientToUpdate);
            
            groupsClient.ClientName = Input.ClientName;
            groupsClient.ClientDescription = Input.ClientDescription;
            await _clientsContext.UpdateAsync(groupsClient.Id, groupsClient);

            return RedirectToPage("./Index", new { organisation = Organisation.Id });
        } 
    }
}