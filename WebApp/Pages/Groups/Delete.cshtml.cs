using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Store;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Groups
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly CustomClientStore _clientStore;
        private readonly GroupService _groupsContext;

        public DeleteModel(CustomClientStore clientStore, GroupService groupsContext)
        {
            _clientStore = clientStore;
            _groupsContext = groupsContext;
        }
        
        [BindProperty]
        public Group Group { get; set; }
        public List<string> Clients { get; set; }
        public string ErrorMessage { get; private set; }
        
        public async Task<IActionResult> OnGetAsync(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Group = _groupsContext.Get(id);
            
            if (Group == null || Group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }

            Clients = new List<string>();

            foreach (var client in _groupsContext.GetClients(Group.Id.ToString()))
            {
                var is4Client = await _clientStore.FindClientByIdAsync(client.ClientId);
                Clients.Add(is4Client.ClientName);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete failed. Try again";
            }

            return Page();
        }
        
        public IActionResult OnPost(string id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Group = _groupsContext.Get(id);
            
            if (Group.OwnerId != User.Claims.First(claim => claim.Type.Equals("sub")).Value)
            {
                return RedirectToPage("./Index");
            }
            
            if (Group == null)
            {
                return RedirectToPage("./Index");
            }

            try
            {
                _groupsContext.Remove(Group);

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("./Delete",
                    new { id, saveChangesError = true });
            }
        }
    }
}