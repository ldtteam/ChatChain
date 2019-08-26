using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class GroupsModel : PageModel
    {
        private readonly GroupService _groupsContext; 
        
        public GroupsModel(GroupService groupsContext)
        {
            _groupsContext = groupsContext;
        }

        public IList<Group> Groups { get; private set; }

        public async Task OnGetAsync()
        {
            Groups = new List<Group>();

            foreach (Group group in await _groupsContext.GetAsync())
            {
                if (group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value) continue;
                
                Groups.Add(group);
            }
        }
    }
}