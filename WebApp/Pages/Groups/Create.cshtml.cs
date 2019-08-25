using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
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
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string groupId = Guid.NewGuid().ToString();

            Group group = new Group
            {
                GroupId = groupId,
                GroupName = Input.GroupName,
                GroupDescription = Input.GroupDescription,
                OwnerId = User.Claims.First(claim => claim.Type.Equals("sub")).Value
            };
            
            await _groupsContext.CreateAsync(group);
            
            return RedirectToPage("./Index");
        }
        
    }
}