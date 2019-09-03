using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Utilities;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly GroupService _groupsContext;
        private readonly OrganisationService _organisationsContext;

        public CreateModel(GroupService groupsContext, OrganisationService organisationsContext)
        {
            _groupsContext = groupsContext;
            _organisationsContext = organisationsContext;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        
        public Organisation Organisation { get; set; }
        
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

        public async Task<IActionResult> OnGetAsync(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.CreateGroups);
            Organisation = org;
            if (!result) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.CreateGroups);
            Organisation = org;
            if (!result) return NotFound();
            
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
                OwnerId = Organisation.Id.ToString()
            };
            
            await _groupsContext.CreateAsync(group);
            
            return RedirectToPage("./Index", new { organisation = Organisation.Id });
        }
        
    }
}