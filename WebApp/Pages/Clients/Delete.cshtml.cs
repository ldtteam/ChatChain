using System;
using System.Threading.Tasks;
using IdentityServer.Store;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClientStore _is4ClientStore;
        private readonly ClientService _clientsContext;

        public DeleteModel(UserManager<ApplicationUser> userManager, CustomClientStore is4ClientStore, ClientService clientsContext)
        {
            _userManager = userManager;
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

            Client = _clientsContext.GetFromClientId(id);
            
            if (Client == null || Client.OwnerId != _userManager.GetUserAsync(User).Result.Id)
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

            var client = await _is4ClientStore.FindClientByIdAsync(id);
            
            var groupClient = _clientsContext.GetFromClientId(id);
            
            if (groupClient != null && groupClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
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