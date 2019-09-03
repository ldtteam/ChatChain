using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using WebApp.Utilities;

namespace WebApp.Pages.Organisations.Users
{
    public class Permissions : PageModel
    {
        
        private readonly OrganisationService _organisationsContext;
        private readonly IdentityServerConnection _identityConfiguration;
        
        public Permissions(OrganisationService organisationsContext, IdentityServerConnection identityConfiguration)
        {
            _organisationsContext = organisationsContext;
            _identityConfiguration = identityConfiguration;
        }
        
        public Organisation Organisation { get; set; }
        
        public ResponseUser RUser { get; set; }
        
        [BindProperty]
        public OrganisationPermissions[] SelectedPermissions { get; set; }
        
        public SelectList PermissionOptions { get; set; }
            
        public class ResponseUser
        {
            public string DisplayName { get; set; }
            public string EmailAddress { get; set; }
            public string Id { get; set; }
        }

        
        public async Task<IActionResult> OnGet(string organisation, string id)
        {
            // Verify user isn't editing themselves
            if (User.Claims.First(claim => claim.Type.Equals("sub")).Value.Equals(id, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditOrgUsers);
            Organisation = org;
            if (!result) return NotFound();

            // Verify user isn't owner
            if (string.Equals(Organisation.Owner, id, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_identityConfiguration.ServerUrl);
                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);

                string usersJson = JsonConvert.SerializeObject(new List<string>{id});
                StringContent contentData = new StringContent(usersJson, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("/api/Users", contentData);
                string stringData = await response.Content.ReadAsStringAsync();
                RUser = JsonConvert.DeserializeObject<IEnumerable<ResponseUser>>(stringData).FirstOrDefault();
            }
            
            PermissionOptions = new SelectList(Enum.GetValues(typeof(OrganisationPermissions)), nameof(OrganisationPermissions), nameof(OrganisationPermissions));

            SelectedPermissions = Organisation.Users[id].Permissions.ToArray();

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string organisation, string id)
        {
            // Verify user isn't editing themselves
            if (User.Claims.First(claim => claim.Type.Equals("sub")).Value.Equals(id, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);
            
            (bool result, Organisation org) = await this.VerifyUserPermissions(organisation, _organisationsContext, OrganisationPermissions.EditOrgUsers);
            Organisation = org;
            if (!result) return NotFound();

            // Verify user isn't owner
            if (string.Equals(Organisation.Owner, id, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403);

            Organisation.Users[id].Permissions = SelectedPermissions;
            await _organisationsContext.UpdateAsync(Organisation.Id, Organisation);
            
            return RedirectToPage("./Index", new { organisation = Organisation.Id } );
        }
    }
}