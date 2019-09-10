using System;
using System.Threading.Tasks;
using Api.Extensions;
using Api.Services;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{organisation}/clients/configs")]
    [Authorize]
    [ApiController]
    public class ClientConfigsController : Controller
    {
        private readonly ClientConfigService _clientConfigService;
        private readonly VerificationService _verificationService;

        public ClientConfigsController(ClientConfigService clientConfigService, VerificationService verificationService)
        {
            _clientConfigService = clientConfigService;
            _verificationService = verificationService;
        }
        
        [HttpGet("{config}", Name = "GetClientConfig")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClientConfig>> GetClientConfigAsync(Guid organisation, Guid config)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ClientConfig apiConfig = await _verificationService.VerifyClientConfig(config);
            ActionResult<bool> result = User.CanGetClientConfig(org, apiConfig);
            return !result.Value ? result.Result : Ok(apiConfig);
        }
        
        [HttpPost("{config}", Name = "UpdateClientConfig")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateClientConfigAsync(Guid organisation, Guid config, ClientConfig clientConfig)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ClientConfig apiConfig = await _verificationService.VerifyClientConfig(config);
            ActionResult<bool> result = User.CanUpdateClientConfig(org, apiConfig);
            if (!result.Value)
                return result.Result;

            clientConfig.Id = apiConfig.Id;
            clientConfig.OwnerId = apiConfig.OwnerId;
            await _clientConfigService.UpdateAsync(apiConfig.Id, clientConfig);

            return Ok();
        }
    }
}