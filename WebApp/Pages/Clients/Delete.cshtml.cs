using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
        public string ErrorMessage { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Client = _clientsContext.Get(id);
            
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

            IdentityServer4.Models.Client client = await _is4ClientStore.FindClientByIdAsync(id);
            
            Client groupClient = _clientsContext.Get(id);
            
            if (groupClient != null && groupClient.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }
            
            if (client == null)
            {
                return NotFound();
            }

            try
            {
                _is4ClientStore.RemoveClient(client);

                if (groupClient != null)
                {
                    _clientsContext.Remove(groupClient);
                }
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("./Delete",
                    new {id, saveChangesError = true });
            }
        }
    }
}