using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConfigurationDbContext _context;

        public EditModel(UserManager<IdentityUser> userManager, ConfigurationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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

            if (Client == null)
            {
                return NotFound();
            }

            Client.ClientId = Client.ClientId.Replace(_userManager.GetUserName(User) + "_", "");

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var clientToUpdate = await _context.Clients.FindAsync(id);
            
            if (await TryUpdateModelAsync<Client>(
                clientToUpdate,
                "client",
                c => c.ClientId , c => c.Enabled))
            {
                if (!clientToUpdate.ClientId.StartsWith(_userManager.GetUserName(User) + "_"))
                {
                    clientToUpdate.ClientId = _userManager.GetUserName(User) + "_" + clientToUpdate.ClientId;
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        } 
    }
}