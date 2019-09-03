using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Utilities;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ClientService _clientsContext;
        private readonly OrganisationService _organisationsContext;
        
        public IndexModel(ClientService clientsContext, OrganisationService organisationsContext)
        {
            _clientsContext = clientsContext;
            _organisationsContext = organisationsContext;
        }
        
        public Organisation Organisation { get; set; }

        public IList<Client> Clients { get; private set; }

        public async Task<IActionResult> OnGetAsync(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyIsMember(organisation, _organisationsContext);
            Organisation = org;
            if (!result) return NotFound();
            
            Clients = new List<Client>();

            foreach (Client client in await _clientsContext.GetFromOwnerIdAsync(Organisation.Id.ToString()))
            {
                Clients.Add(client);
            }

            return Page();
        }
    }
}