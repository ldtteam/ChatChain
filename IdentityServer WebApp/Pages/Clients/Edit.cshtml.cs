using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConfigurationDbContext _context;
        private readonly GroupsDbContext _groupsContext;

        public EditModel(UserManager<IdentityUser> userManager, ConfigurationDbContext context, GroupsDbContext groupsContext)
        {
            _userManager = userManager;
            _context = context;
            _groupsContext = groupsContext;
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

            var groupsClient = await _groupsContext.Clients.FirstAsync(c => c.ClientId == id);

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
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var groupsClient = await _groupsContext.Clients.FirstAsync(c => c.ClientId == id);

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
                return RedirectToPage("./Index");
            }

            return Page();
        } 
    }
}