using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Store;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer_WebApp.Models;
using IdentityServer_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Client = IdentityServer4.Models.Client;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer_WebApp.Pages.Clients
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClientStore _clientStore;
        private readonly ClientService _clientsContext;

        public CreateModel(UserManager<ApplicationUser> userManager, CustomClientStore clientStore, ClientService clientsContext)
        {
            _userManager = userManager;
            _clientStore = clientStore;
            _clientsContext = clientsContext;
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

            var client = new Client
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
                    "ChatChain"
                },
                
                AllowOfflineAccess = true
            };
            
            //_is4Context.Clients.Add(emptyClient);
            //await _is4Context.SaveChangesAsync();

            _clientStore.AddClient(client);
            
            /*Client is4Client = await _is4Context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == clientId)*/;

            var is4Client = await _clientStore.FindClientByIdAsync(clientId);

            var newClient = new Models.Client
            {
                OwnerId = _userManager.GetUserAsync(User).Result.Id,
                ClientId = is4Client.ClientId,
                ClientGuid = is4Client.ClientId,
                ClientName = is4Client.ClientName
            };

            _clientsContext.Create(newClient);
            //await _clientsContext.SaveChangesAsync();
            
            return RedirectToPage("./Index");
        }
        
    }
}