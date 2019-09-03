using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.RandomGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Utilities;

namespace WebApp.Pages.Organisations.Invite
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
        
        [TempData] 
        // ReSharper disable once MemberCanBePrivate.Global
        public string Token { get; set; }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class InputModel
        {
            [Required]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "User Email")]
            public string UserEmail { get; set; }
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public Organisation Organisation { get; private set; }
        
        public async Task<IActionResult> OnGetAsync(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.CreateOrgUsers);
            Organisation = org;
            if (!result) return NotFound();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string organisation)
        {
            if (Input.UserEmail == null) return Page();
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.CreateOrgUsers);
            Organisation = org;
            if (!result) return NotFound();
            
            Token = PasswordGenerator.Generate();
            
            OrganisationInvite invite = new OrganisationInvite
            {
                OrganisationId = Organisation.Id.ToString(),
                Token = Token,
                Email = Input.UserEmail
            };

            await _organisationsContext.CreateInviteAsync(invite);

            return RedirectToPage("./Show");

        }
    }
}