using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Organisations
{
    [Authorize]
    public class Create : PageModel
    {
        private readonly OrganisationService _organisationsContext;

        public Create(OrganisationService organisationsContext)
        {
            _organisationsContext = organisationsContext;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Organisation Name")]
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Organisation organisation = new Organisation
            {
                Name = Input.Name,
                Owner = User.Claims.First(claim => claim.Type.Equals("sub")).Value,
                Users = new Dictionary<string, OrganisationUser>
                {
                    { 
                        User.Claims.First(claim => claim.Type.Equals("sub")).Value,
                        new OrganisationUser
                        {
                            Permissions = new List<OrganisationPermissions>
                            {
                                OrganisationPermissions.All
                            }
                        }
                    }
                }
            };

            await _organisationsContext.CreateAsync(organisation);

            return RedirectToPage("./Index");

        }
    }
}