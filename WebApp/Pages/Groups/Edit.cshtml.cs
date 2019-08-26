using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly GroupService _groupsContext;

        public EditModel(GroupService groupContext)
        {
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
            
            [Required]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Group Description")]
            public string GroupDescription { get; set; }
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Group = await _groupsContext.GetAsync(new ObjectId(id));
            
            if (Group == null || Group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }

            Input = new InputModel
            {
                GroupName = Group.GroupName,
                GroupDescription = Group.GroupDescription
            };

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            Group = await _groupsContext.GetAsync(new ObjectId(id));
            
            if (Group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }

            Group.GroupName = Input.GroupName;
            Group.GroupDescription = Input.GroupDescription;
            
            await _groupsContext.UpdateAsync(Group.Id, Group);
            
            return RedirectToPage("./Index");

        } 
    }
}