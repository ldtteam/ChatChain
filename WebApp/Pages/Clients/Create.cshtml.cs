using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerStore;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Util;
using Client = IdentityServer4.Models.Client;
using Secret = IdentityServer4.Models.Secret;

namespace WebApp.Pages.Clients
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly CustomClientStore _clientStore;
        private readonly ClientService _clientsContext;

        public CreateModel(CustomClientStore clientStore, ClientService clientsContext)
        {
            _clientStore = clientStore;
            _clientsContext = clientsContext;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        
        [TempData]
        public string StatusMessage { get; set; }


        public class InputModel
        {
            
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Client Name")]
            public string ClientName { get; set; }
            
            [Required]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Client Description")]
            public string ClientDescription { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "\nThe password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            Input = new InputModel {Password = PasswordGenerator.Generate()};
            StatusMessage = $"Client password is: {Input.Password}\n You Will Not Receive This Again!";
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string clientId = Guid.NewGuid().ToString();

            Client client = new Client
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

            _clientStore.AddClient(client);
            
            Client is4Client = await _clientStore.FindClientByIdAsync(clientId);

            ChatChainCommon.DatabaseModels.Client newClient = new ChatChainCommon.DatabaseModels.Client
            {
                //OwnerId = _userManager.GetUserAsync(User).Result.Id,
                OwnerId = User.Claims.First(claim => claim.Type.Equals("sub")).Value,
                ClientId = is4Client.ClientId,
                ClientGuid = is4Client.ClientId,
                ClientName = is4Client.ClientName,
                ClientDescription = Input.ClientDescription
            };

            await _clientsContext.CreateAsync(newClient);
            
            return RedirectToPage("./Index");
        }
        
    }
}