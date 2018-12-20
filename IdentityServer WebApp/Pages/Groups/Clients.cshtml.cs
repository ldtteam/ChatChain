using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class ClientsModel : PageModel
    {
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupsDbContext _groupsContext;
        
        public ClientsModel(ConfigurationDbContext is4Context, GroupsDbContext groupsContext)
        {
            _is4Context = is4Context;
            _groupsContext = groupsContext;
        }

        public IList<Client> Clients { get; set; }
        public Group Group { get; set; }
        public List<int> ClientIds { get; set;  }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Clients = await _is4Context.Clients.ToListAsync();
            
            if (id == null)
            {
                return NotFound();
            }

            Group = await _groupsContext.Groups.Include(group => group.Clients).FirstAsync(group => group.Id == id);

            if (Group == null)
            {
                return NotFound();
            }

            ClientIds = new List<int>();
            
            foreach (var client in Group.Clients)
            {
                ClientIds.Add(client.ClientId);
            }
            
            return Page();
        }
    }
}