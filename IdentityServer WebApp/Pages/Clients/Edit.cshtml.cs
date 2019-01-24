using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer_WebApp.Data;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConfigurationDbContext _context;
        private readonly ClientService _clientsContext;

        public EditModel(UserManager<IdentityUser> userManager, ConfigurationDbContext context, ClientService clientsContext)
        {
            _userManager = userManager;
            _context = context;
            _clientsContext = clientsContext;
        }
        
        [BindProperty]
        public Client Client { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Client = await _context.Clients.FindAsync(id);

            var groupsClient = _clientsContext.GetFromClientId(id.Value);

            if (groupsClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            if (Client == null)
            {
                return NotFound();
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var groupsClient = _clientsContext.GetFromClientId(id.Value);

            if (groupsClient.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }

            var clientToUpdate = await _context.Clients.FindAsync(id);
            
            if (await TryUpdateModelAsync<Client>(
                clientToUpdate,
                "client",
                c => c.ClientName , c => c.Enabled))
            {
                await _context.SaveChangesAsync();
                groupsClient.ClientName = clientToUpdate.ClientName;
                _clientsContext.Update(groupsClient.Id.ToString(), groupsClient);
                return RedirectToPage("./Index");
            }

            return Page();
        } 
    }
}