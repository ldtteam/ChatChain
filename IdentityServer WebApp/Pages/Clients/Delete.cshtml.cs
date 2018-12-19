using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupsDbContext _groupsContext;

        public DeleteModel(ConfigurationDbContext context, GroupsDbContext groupsContext)
        {
            _is4Context = context;
            _groupsContext = groupsContext;
        }
        
        [BindProperty]
        public Client Client { get; set; }
        public string ErrorMessage { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Client = await _is4Context.Clients.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (Client == null)
            {
                return NotFound();
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

            var client = await _is4Context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            var groupClient = await _groupsContext.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ClientId == id);
            
            if (client == null || groupClient == null)
            {
                return NotFound();
            }

            try
            {
                _is4Context.Clients.Remove(client);
                await _is4Context.SaveChangesAsync();

                _groupsContext.Clients.Remove(groupClient);
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