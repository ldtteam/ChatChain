using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Extensions;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ConfigurationDbContext _is4Context;
        private readonly ClientService _clientsContext;

        public DeleteModel(UserManager<ApplicationUser> userManager, ConfigurationDbContext context, ClientService clientsContext)
        {
            _userManager = userManager;
            _is4Context = context;
            _clientsContext = clientsContext;
        }
        
        [BindProperty]
        public Client Client { get; set; }
        public string ErrorMessage { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Client = await _is4Context.Clients.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            var groupsClient = _clientsContext.GetFromClientId(id.Value);
            
            Console.WriteLine("Get Subject Id: " + _userManager.GetUserAsync(User).Result.Id);
            
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
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _is4Context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            var groupClient = _clientsContext.GetFromClientId(id.Value);
            
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
                _is4Context.Clients.Remove(client);
                await _is4Context.SaveChangesAsync();

                if (groupClient != null)
                {
                    _clientsContext.Remove(groupClient);
                }
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("./Delete",
                    new { id = id, saveChangesError = true });
            }
        }
    }
}