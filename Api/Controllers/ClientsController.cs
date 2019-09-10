using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Extensions;
using Api.Models;
using Api.Services;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.IdentityServerStore;
using ChatChainCommon.RandomGenerator;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Client = ChatChainCommon.DatabaseModels.Client;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{organisation}/clients")]
    [Authorize]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly ClientService _clientService;
        private readonly CustomClientStore _is4ClientService;
        private readonly ClientConfigService _clientConfigService;
        private readonly VerificationService _verificationService;

        public ClientsController(ClientService clientService, CustomClientStore is4ClientService, ClientConfigService clientConfigService, VerificationService verificationService)
        {
            _clientService = clientService;
            _is4ClientService = is4ClientService;
            _clientConfigService = clientConfigService;
            _verificationService = verificationService;
        }
        
        [HttpGet("", Name = "GetClients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsAsync(Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanGetClients(org);
            return !result.Value ? result.Result : Ok(await _clientService.GetFromOwnerIdAsync(org.Id));
        }
        
        [HttpGet("{client}", Name = "GetClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Client>> GetClientAsync(Guid organisation, Guid client)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Client apiClient = await _verificationService.VerifyClient(client);
            ActionResult<bool> result = User.CanGetClient(org, apiClient);
            return !result.Value ? result.Result : Ok(apiClient);
        }
        
        [HttpPost("", Name = "CreateClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CreateClientResponse>> CreateClientAsync(Guid organisation, Client client)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanCreateClient(org);
            if (!result.Value)
                return result.Result;

            client.Id = Guid.NewGuid(); //Makes certain that the api client can't set a specific ID in the database (would cause possible issues)
            client.OwnerId = org.Id;

            string password = PasswordGenerator.Generate();
            
            IdentityServer4.Models.Client is4Client = new IdentityServer4.Models.Client
            {
                ClientId = client.Id.ToString(),
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                //Client secrets
                ClientSecrets =
                {
                    new Secret(password.Sha256())
                },
                
                AllowedScopes =
                {
                    "ChatChain"
                },
                
                AllowOfflineAccess = true
            };
            _is4ClientService.AddClient(is4Client);
            await _clientService.CreateAsync(client);
            
            await _clientConfigService.CreateAsync(new ClientConfig
            {
                Id = client.Id,
                OwnerId = org.Id
            });
            
            CreateClientResponse response = new CreateClientResponse
            {
                Password = password,
                Id = client.Id
            };

            return !result.Value ? result.Result : Ok(response);
        }
        
        [HttpPost("{client}", Name = "UpdateClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateClientAsync(Guid organisation, Guid client, Client updateClient)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Client apiClient = await _verificationService.VerifyClient(client);
            ActionResult<bool> result = User.CanUpdateClient(org, apiClient);
            if (!result.Value)
                return result.Result;

            updateClient.Id = apiClient.Id;
            updateClient.OwnerId = apiClient.OwnerId;
            await _clientService.UpdateAsync(apiClient.Id, updateClient);

            return !result.Value ? result.Result : Ok();
        }
        
        [HttpDelete("{client}", Name = "DeleteClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteClientAsync(Guid organisation, Guid client)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Client apiClient = await _verificationService.VerifyClient(client);
            ActionResult<bool> result = User.CanDeleteClient(org, apiClient);
            if (!result.Value)
                return result.Result;
            
            _is4ClientService.RemoveClient(apiClient.Id.ToString());
            await _clientService.RemoveAsync(apiClient.Id);
            await _clientConfigService.RemoveAsync(apiClient.Id);

            return !result.Value ? result.Result : Ok();
        }
    }
}