using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ConfigurationDbContext _is4Context;
        public readonly ClientService ClientsContext;
        
        public IndexModel(UserManager<ApplicationUser> userManager, ConfigurationDbContext is4Context, ClientService clientsContext)
        {
            _userManager = userManager;
            _is4Context = is4Context;
            ClientsContext = clientsContext;
        }

        public IList<Client> Clients { get; set; }

        public async Task OnGetAsync()
        {
            Clients = new List<Client>();

            foreach (var client in await _is4Context.Clients.ToListAsync())
            {
                var groupClient = ClientsContext.GetFromClientId(client.Id);

                if (groupClient != null && groupClient.OwnerId == _userManager.GetUserAsync(User).Result.Id)
                {
                    Clients.Add(client);
                }
            }
        }
        //d10d988b-e7f4-4c40-bf86-c1beeaded8b3; 38; Test Client 1; 12/24/2018 19:07:28
    }
}