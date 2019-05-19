using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly GroupService _groupsContext;

        public CreateModel(GroupService groupsContext)
        {
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
                OwnerId = User.Claims.First(claim => claim.Type.Equals("sub")).Value
            };
            
            _groupsContext.Create(group);
            
            return RedirectToPage("./Index");
        }
        
    }
}