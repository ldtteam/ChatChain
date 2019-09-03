using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using WebApp.Utilities;
using Client = IdentityServer4.Models.Client;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly CustomClientStore _clientStore;
        private readonly GroupService _groupsContext;
        private readonly OrganisationService _organisationsContext; 

        public DeleteModel(CustomClientStore clientStore, GroupService groupsContext, OrganisationService organisationsContext)
        {
            _clientStore = clientStore;
            _groupsContext = groupsContext;
            _organisationsContext = organisationsContext;
        }
        
        [BindProperty]
        public Group Group { get; set; }
        public List<string> Clients { get; private set; }
        public string ErrorMessage { get; private set; }
        
        public Organisation Organisation { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string organisation, string group, bool? saveChangesError = false)
        {
            if (group == null)
            {
                return RedirectToPage("./Index");
            }
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.DeleteGroups);
            Organisation = org;
            if (!result) return NotFound();

            Group = await _groupsContext.GetAsync(new ObjectId(group));
            
            if (Group == null || Group.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index");
            }

            Clients = new List<string>();

            foreach (ChatChainCommon.DatabaseModels.Client client in await _groupsContext.GetClientsAsync(Group.Id))
            {
                Client is4Client = await _clientStore.FindClientByIdAsync(client.ClientId);
                Clients.Add(is4Client.ClientName);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete failed. Try again";
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPost(string organisation, string group)
        {
            if (group == null)
            {
                return RedirectToPage("./Index");
            }
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.DeleteGroups);
            Organisation = org;
            if (!result) return NotFound();

            Group = await _groupsContext.GetAsync(new ObjectId(group));
            
            if (Group.OwnerId != Organisation.Id.ToString())
            {
                return RedirectToPage("./Index");
            }
            
            if (Group == null)
            {
                return RedirectToPage("./Index");
            }

            try
            {
                await _groupsContext.RemoveAsync(Group.Id);

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("./Delete",
                    new { group, saveChangesError = true });
            }
        }
    }
}