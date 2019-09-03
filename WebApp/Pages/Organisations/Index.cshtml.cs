using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Organisations
{
    [Authorize]
    public class Index : PageModel
    {
        private readonly OrganisationService _organisationsContext;

        public Index(OrganisationService organisationsContext)
        {
            _organisationsContext = organisationsContext;
        }
        
        public IList<Organisation> Organisations { get; private set; }

        public async Task OnGet()
        {
            Organisations = new List<Organisation>();

            foreach (Organisation organisation in await _organisationsContext.GetForUserAsync(User.Claims.First(claim => claim.Type.Equals("sub")).Value))
            {
                Organisations.Add(organisation);
            }
        }
    }
}