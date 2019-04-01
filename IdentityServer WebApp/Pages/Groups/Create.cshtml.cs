using System;
using System.ComponentModel.DataAnnotations;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GroupService _groupsContext;

        public CreateModel(UserManager<ApplicationUser> userManager, GroupService groupsContext)
        {
            _userManager = userManager;
            _groupsContext = groupsContext;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Group Name")]
            public string GroupName { get; set; }
            
            [Required]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Group Description")]
            public string GroupDescription { get; set; }
        }
        
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var groupId = Guid.NewGuid().ToString();

            var group = new Group
            {
                GroupId = groupId,
                GroupName = Input.GroupName,
                GroupDescription = Input.GroupDescription,
                OwnerId = _userManager.GetUserAsync(User).Result.Id
            };
            
            _groupsContext.Create(group);
            
            return RedirectToPage("./Index");
        }
        
    }
}