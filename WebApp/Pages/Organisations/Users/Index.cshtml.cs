using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebApp.Utilities;

namespace WebApp.Pages.Organisations.Users
{
    public class Index : PageModel
    {
        private readonly OrganisationService _organisationsContext;
        private readonly IdentityServerConnection _identityConfiguration;
        
        public Index(OrganisationService organisationsContext, IdentityServerConnection identityConfiguration)
        {
            _organisationsContext = organisationsContext;
            _identityConfiguration = identityConfiguration;
        }

        public IEnumerable<ResponseUser> Users { get; set; }
        
        public Organisation Organisation { get; set; }
        
        public class ResponseUser
        {
            public string DisplayName { get; set; }
            public string EmailAddress { get; set; }
            public string Id { get; set; }
        }

        public async Task<IActionResult> OnGet(string organisation)
        {
            (bool result, Organisation org) = await this.VerifyIsMember(organisation, _organisationsContext);
            Organisation = org;
            if (!result) return NotFound();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_identityConfiguration.ServerUrl);
                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);

                string usersJson = JsonConvert.SerializeObject(Organisation.Users.Keys);
                StringContent contentData = new StringContent(usersJson, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("/api/Users", contentData);
                string stringData = await response.Content.ReadAsStringAsync();
                Users = JsonConvert.DeserializeObject<IEnumerable<ResponseUser>>(stringData);
            }

            return Page();
        }
    }
}