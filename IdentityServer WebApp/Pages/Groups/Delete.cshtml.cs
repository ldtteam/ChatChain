using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Extensions;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupsDbContext _groupsContext;

        public DeleteModel(ConfigurationDbContext is4Context, GroupsDbContext groupsContext)
        {
            _is4Context = is4Context;
            _groupsContext = groupsContext;
        }
        
        [BindProperty]
        public Group Group { get; set; }
        public List<String> Clients { get; set; }
        public string ErrorMessage { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Group = await _groupsContext.Groups.FirstOrDefaultAsync(g => g.Id == id);

            if (Group == null)
            {
                return NotFound();
            }

            var testClient = await _groupsContext.Clients.FirstAsync(c => c.Id == 12);
            var testClient2 = await _groupsContext.Clients.FirstAsync(c => c.Id == 11);
            
            Console.WriteLine($"Test Client: {testClient}");

            /*if (Group.Clients == null)
            {
                Group.Clients = new List<Data.Client>();
            }*/

            if (testClient != null)
            {
                Console.WriteLine($"Group Clients: {Group.Clients == null}");
                Group.Clients.Add(testClient);
                Group.Clients.Add(testClient2);
            }

            Clients = new List<string>();

            if (!Group.Clients.IsNullOrEmpty())
            {
                foreach (var client in Group.Clients)
                {
                    var is4Client = await _is4Context.Clients.FirstAsync(c => c.Id == client.ClientId);
                    Clients.Add(is4Client.ClientName);
                }
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete failed. Try again";
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _groupsContext.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (group == null)
            {
                return NotFound();
            }

            try
            {
                _groupsContext.Groups.Remove(group);
                await _groupsContext.SaveChangesAsync();
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