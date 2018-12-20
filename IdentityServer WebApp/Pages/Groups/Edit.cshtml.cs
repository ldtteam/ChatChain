using System.Threading.Tasks;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GroupsDbContext _groupsContext;

        public EditModel(UserManager<IdentityUser> userManager, GroupsDbContext groupContext)
        {
            _userManager = userManager;
            _groupsContext = groupContext;
        }
        
        [BindProperty]
        public Group Group { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Group = await _groupsContext.Groups.FindAsync(id);

            if (Group == null)
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

            var groupToUpdate = await _groupsContext.Groups.FindAsync(id);

            if (!await TryUpdateModelAsync<Group>(
                groupToUpdate,
                "group",
                g => g.GroupName)) return Page();
            await _groupsContext.SaveChangesAsync();
            return RedirectToPage("./Index");

        } 
    }
}