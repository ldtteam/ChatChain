using System.Collections.Generic;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GroupService _groupsContext; 
        
        public GroupsModel(UserManager<ApplicationUser> userManager, GroupService groupsContext)
        {
            _userManager = userManager;
            _groupsContext = groupsContext;
        }

        public IList<Group> Groups { get; set; }

        public void OnGet()
        {
            Groups = new List<Group>();

            foreach (var group in _groupsContext.Get())
            {
                if (group.OwnerId != _userManager.GetUserAsync(User).Result.Id) continue;
                
                Groups.Add(group);
            }
        }
    }
}