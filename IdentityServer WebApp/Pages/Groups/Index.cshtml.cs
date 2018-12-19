using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class GroupsModel : PageModel
    {
        private readonly GroupsDbContext _groupsContext;
        
        public GroupsModel(GroupsDbContext groupsContext)
        {
            _groupsContext = groupsContext;
        }

        public IList<Group> Groups { get; set; }

        public async Task OnGetAsync()
        {
            Groups = await _groupsContext.Groups.ToListAsync();
        }
    }
}