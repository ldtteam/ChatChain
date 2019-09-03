using System;
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
    public class DeleteModel : PageModel
    {
        private readonly CustomClientStore _is4ClientStore;
        private readonly ClientService _clientsContext;
        private readonly OrganisationService _organisationsContext;

        public DeleteModel(CustomClientStore is4ClientStore, ClientService clientsContext, OrganisationService organisationsContext)
        {
            _is4ClientStore = is4ClientStore;
            _clientsContext = clientsContext;
            _organisationsContext = organisationsContext;
        }
        
        [BindProperty]
        public Client Client { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string ErrorMessage { get; set; }
        
        public Organisation Organisation { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string organisation, string client)
        {
            if (client == null)
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.DeleteClients);
            Organisation = org;
            if (!result) return NotFound();

            Client = await _clientsContext.GetAsync(new ObjectId(client));
            
            if (Client == null || Client.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string organisation, string client)
        {
            if (client == null)
            {
                return NotFound();
            }
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.DeleteClients);
            Organisation = org;
            if (!result) return NotFound();

            Client groupClient = await _clientsContext.GetAsync(new ObjectId(client));

            if (groupClient == null)
            {
                return NotFound();
            }
            
            if (groupClient.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index", new { organisation = Organisation.Id });
            }

            IdentityServer4.Models.Client is4Client = await _is4ClientStore.FindClientByIdAsync(groupClient.ClientId);
            
            if (is4Client == null)
            {
                return NotFound();
            }

            _is4ClientStore.RemoveClient(is4Client);
            await _clientsContext.RemoveAsync(groupClient.Id);
            return RedirectToPage("./Index", new { organisation = Organisation.Id });
        }
    }
}