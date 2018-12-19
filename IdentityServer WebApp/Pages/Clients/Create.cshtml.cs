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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupsDbContext _groupsContext;

        public CreateModel(UserManager<IdentityUser> userManager, ConfigurationDbContext is4Context, GroupsDbContext groupsContext)
        {
            _userManager = userManager;
            _is4Context = is4Context;
            _groupsContext = groupsContext;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            
            [Required]
            //[StringLength(25, ErrorMessage = "The {0] must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [DataType(DataType.Text)]
            [Display(Name = "Client Name")]
            public string ClientName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var clientId = Guid.NewGuid().ToString();

            var emptyClient = new IdentityServer4.Models.Client
            {
                ClientId = clientId,
                ClientName = Input.ClientName,
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                //Client secrets
                ClientSecrets =
                {
                    new Secret(Input.Password.Sha256())
                },
                AllowedScopes =
                {
                    "api1"
                },
                AllowOfflineAccess = true
            }.ToEntity();
            
            _is4Context.Clients.Add(emptyClient);
            await _is4Context.SaveChangesAsync();

            Client is4Client = await _is4Context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            var newClient = new Data.Client
            {
                OwnerId = _userManager.GetUserAsync(User).Id,
                ClientId = is4Client.Id
            };

            _groupsContext.Clients.Add(newClient);
            await _groupsContext.SaveChangesAsync();
            
            /*var group = new Group();
            
            group.OwnerId = _userManager.GetUserAsync(User).Id;
            group.GroupName = "Test";
            
            var client = await _groupsContext.Clients
                .FirstOrDefaultAsync(c => c.ClientId == is4Client.Id);
            
            Console.WriteLine($"Client: {client.Id} {client.ClientId} {client.OwnerId}");
            
            group.Clients.Add(client);

            _groupsContext.Groups.Add(group);
            await _groupsContext.SaveChangesAsync();*/
            
            return RedirectToPage("./Index");
        }
        
    }
}