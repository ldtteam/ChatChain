using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Utilities;

namespace WebApp.Pages.Organisations
{
    public class Edit : PageModel
    {
        private readonly OrganisationService _organisationsContext;

        public Edit(OrganisationService organisationsContext)
        {
            _organisationsContext = organisationsContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        
        public Organisation Organisation { get; set; }
        
        // ReSharper disable once ClassNeverInstantiated.Global
        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Organisation Name")]
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditOrg);
            Organisation = org;
            if (!result) return NotFound();
            
            Input = new InputModel
            {
                Name = Organisation.Name
            };
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string organisation)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditOrg);
            Organisation = org;
            if (!result) return NotFound();
            
            if (Organisation.Name == Input.Name)
                return RedirectToPage("./Index");

            Organisation.Name = Input.Name;
            await _organisationsContext.UpdateAsync(Organisation.Id, Organisation);

            return RedirectToPage("./Index");
        }
    }
}