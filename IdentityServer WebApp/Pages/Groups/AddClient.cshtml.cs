using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class AddClientModel : PageModel
    {
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupsDbContext _groupsContext;

        public AddClientModel(ConfigurationDbContext is4Context, GroupsDbContext groupsContext)
        {
            _is4Context = is4Context;
            _groupsContext = groupsContext;
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
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Group = await _groupsContext.Groups.Include(group => group.Clients).FirstAsync(g => g.Id == id);

            if (Group == null)
            {
                return NotFound();
            }
            
            var clients = new List<SelectListItem>();

            foreach (var client in _groupsContext.Clients)
            {

                if (!Group.Clients.Contains(client))
                {
                    var is4Client = await _is4Context.Clients.FirstAsync(c => c.Id == client.ClientId);

                    clients.Add(new SelectListItem
                    {
                        Text = is4Client.ClientName,
                        Value = is4Client.Id.ToString()
                    });
                }
            }

            Clients = clients;
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Console.WriteLine($"ClientId: {Input.ClientId}");

            Group = await _groupsContext.Groups.Include(group => group.Clients).FirstAsync(g => g.Id == id);
            var client = await _groupsContext.Clients.FirstAsync(c => c.ClientId == int.Parse(Input.ClientId));

            Console.WriteLine($"Client: {client}");
            Console.WriteLine($"Thingy: {Group == null}");
            
            Group.Clients.Add(client);
            await _groupsContext.SaveChangesAsync();
            
            return RedirectToPage("./Clients");
        }
        
    }
}