using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using WebApp.Utilities;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly GroupService _groupsContext;
        private readonly OrganisationService _organisationsContext; 

        public EditModel(GroupService groupContext, OrganisationService organisationsContext)
        {
            _groupsContext = groupContext;
            _organisationsContext = organisationsContext;
        }
        
        public Group Group { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        
        public Organisation Organisation { get; set; }
        
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
        
        public async Task<IActionResult> OnGetAsync(string organisation, string group)
        {
            if (group == null)
            {
                return RedirectToPage("./Index");
            }

            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditGroups);
            Organisation = org;
            if (!result) return NotFound();

            Group = await _groupsContext.GetAsync(new ObjectId(group));
            
            if (Group == null || Group.OwnerId != Organisation.Id.ToString())
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
        
        public async Task<IActionResult> OnPostAsync(string organisation, string group)
        {
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditGroups);
            Organisation = org;
            if (!result) return NotFound();
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Group = await _groupsContext.GetAsync(new ObjectId(group));
            
            if (Group.OwnerId != Organisation.Id.ToString())
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