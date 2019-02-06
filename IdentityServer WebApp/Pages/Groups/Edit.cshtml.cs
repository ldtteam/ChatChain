using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GroupService _groupsContext;

        public EditModel(UserManager<ApplicationUser> userManager, GroupService groupContext)
        {
            _userManager = userManager;
            _groupsContext = groupContext;
        }
        
        public Group Group { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            [Display(Name = "Group Name")]
            public string GroupName { get; set; }
        }
        
        public IActionResult OnGet(string id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Group = _groupsContext.Get(id);
            
            if (Group == null || Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }

            Input = new InputModel
            {
                GroupName = Group.GroupName
            };

            return Page();
        }
        
        public IActionResult OnPost(string id)
        {
            Console.WriteLine("test123");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Console.WriteLine("test1234");
            
            Group = _groupsContext.Get(id);
            
            if (Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            Console.WriteLine("test1235");

            var groupToUpdate = _groupsContext.Get(id);

            groupToUpdate.GroupName = Input.GroupName;

            Console.WriteLine($"Group Name: {groupToUpdate.GroupName}");
            _groupsContext.Update(groupToUpdate.Id.ToString(), groupToUpdate);
            
            return RedirectToPage("./Index");

        } 
    }
}