using System;
using System.Threading.Tasks;
using IdentityServer.Store;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.Models.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClientStore _clientStore;
        private readonly ClientService _clientsContext;

        public DeleteModel(UserManager<ApplicationUser> userManager, CustomClientStore clientStore, ClientService clientsContext)
        {
            _userManager = userManager;
            _clientStore = clientStore;
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

            Client = await _clientStore.FindClientByIdAsync(id);
            
            var groupsClient = _clientsContext.GetFromClientId(id);
            
            Console.WriteLine("Get Subject Id: " + _userManager.GetUserAsync(User).Result.Id);
            Console.WriteLine("Subject: " + groupsClient.OwnerId);
            Console.WriteLine("Client: " + Client);
            
            if (groupsClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            if (Client == null)
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

            var client = await _clientStore.FindClientByIdAsync(id);
            
            var groupClient = _clientsContext.GetFromClientId(id);

            Console.WriteLine(groupClient != null);
            
            if (groupClient != null && groupClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                Console.WriteLine(groupClient.OwnerId != _userManager.GetUserAsync(User).Result.Id);
                return RedirectToPage("./Index");
            }
            
            if (client == null)
            {
                return NotFound();
            }

            try
            {
                _clientStore.RemoveClient(client);

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