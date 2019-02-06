using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class AddClientModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;

        public AddClientModel(UserManager<ApplicationUser> userManager, ConfigurationDbContext is4Context, GroupService groupsContext, ClientService clientsContext)
        {
            _userManager = userManager;
            _is4Context = is4Context;
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
        }

        public Group Group { get; set; }
        public IEnumerable<SelectListItem> Clients { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            [Required]
            [Display(Name = "Client")]
            public string ClientId { get; set; }
        }
        
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("./Clients");
            }

            Group = _groupsContext.Get(id);

            if (Group == null || Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Clients");
            }
            
            var clients = new List<SelectListItem>();

            var clientIds = new List<string>();

            foreach (var client in _groupsContext.GetClients(Group.Id.ToString()))
            {
                clientIds.Add(client.Id.ToString());
            }

            foreach (var client in _clientsContext.GetFromOwnerId(_userManager.GetUserAsync(User).Result.Id))
            {
                //if (!_clientsContext.GetGroups(client.Id.ToString()).Contains(Group)) continue;

                if (!clientIds.Contains(client.Id.ToString()))
                {
                    var is4Client = await _is4Context.Clients.FirstOrDefaultAsync(c => c.Id == client.ClientId);

                    if (is4Client != null)
                    {
                        clients.Add(new SelectListItem
                        {
                            Text = is4Client.ClientName,
                            Value = is4Client.Id.ToString()
                        });
                    }
                }
            }

            Clients = clients;
            
            return Page();
        }
        
        public IActionResult OnPost(string id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Group = _groupsContext.Get(id);
            var client = _clientsContext.GetFromClientId(int.Parse(Input.ClientId));

            if (Group != null)
            {
                //_groupsContext.AddClient(Group.Id, client.Id);
                //_clientsContext.AddGroup(client.Id, Group.Id);
                _groupsContext.AddClient(Group.Id, client.Id);
                return RedirectToPage("./Clients", new { id = Group.Id} );
            }

            return RedirectToPage("./Index");
        }
    }
}