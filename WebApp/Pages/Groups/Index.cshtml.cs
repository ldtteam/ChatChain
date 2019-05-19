using System.Collections.Generic;
using System.Linq;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        public IList<Group> Groups { get; set; }

        public void OnGet()
        {
            Groups = new List<Group>();

            foreach (var group in _groupsContext.Get())
            {
                if (group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value) continue;
                
                Groups.Add(group);
            }
        }
    }
}