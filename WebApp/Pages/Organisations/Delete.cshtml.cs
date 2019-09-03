using System;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Utilities;

namespace WebApp.Pages.Organisations
{
    public class Delete : PageModel
    {
        private readonly OrganisationService _organisationsContext;
        
        public Delete(OrganisationService organisationsContext)
        {
            _organisationsContext = organisationsContext;
        }
        
        public Organisation Organisation { get; set; }

        public async Task<IActionResult> OnGet(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyIsOwner(organisation, _organisationsContext);
            Organisation = org;
            
            if (result)
            {
                return Page();
            }
            return NotFound();
        }
        
        public async Task<IActionResult> OnPost(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyIsOwner(organisation, _organisationsContext);
            Organisation = org;

            if (!result) return NotFound();
            
            await _organisationsContext.RemoveAsync(Organisation.Id);

            return RedirectToPage("./Index");
        }
    }
}