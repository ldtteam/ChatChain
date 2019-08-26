using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly CustomClientStore _is4ClientStore;
        private readonly ClientService _clientsContext;

        public DeleteModel(CustomClientStore is4ClientStore, ClientService clientsContext)
        {
            _is4ClientStore = is4ClientStore;
            _clientsContext = clientsContext;
        }
        
        [BindProperty]
        public Client Client { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string ErrorMessage { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Client = await _clientsContext.GetAsync(new ObjectId(id));
            
            if (Client == null || Client.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Client groupClient = await _clientsContext.GetAsync(new ObjectId(id));

            if (groupClient == null)
            {
                return NotFound();
            }
            
            if (groupClient.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }

            IdentityServer4.Models.Client client = await _is4ClientStore.FindClientByIdAsync(groupClient.ClientId);
            
            if (client == null)
            {
                return NotFound();
            }

            _is4ClientStore.RemoveClient(client);
            await _clientsContext.RemoveAsync(groupClient.Id);
            return RedirectToPage("./Index");
        }
    }
}