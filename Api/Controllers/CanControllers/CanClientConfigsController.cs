using System;
using System.Threading.Tasks;
using Api.Extensions;
using Api.Services;
using ChatChainCommon.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.CanControllers
{
    [Produces("application/json")]
    [Route("api/can/{respond200}/{organisation}/clients/configs")]
    [Authorize]
    [ApiController]
    public class CanClientConfigsController : Controller
    {
        private readonly VerificationService _verificationService;

        public CanClientConfigsController(VerificationService verificationService)
        {
            _verificationService = verificationService;
        }
        
        [HttpGet("{config}", Name = "CanGetClientConfig")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetClientConfigAsync(bool respond200, Guid organisation, Guid config)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ClientConfig apiConfig = await _verificationService.VerifyClientConfig(config);
            ActionResult<bool> result = User.CanGetClientConfig(org, apiConfig);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{config}/update", Name = "CanUpdateClientConfig")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanUpdateClientConfigAsync(bool respond200, Guid organisation, Guid config)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ClientConfig apiConfig = await _verificationService.VerifyClientConfig(config);
            ActionResult<bool> result = User.CanUpdateClientConfig(org, apiConfig);
            return respond200 ? Ok(result.Value) : result;
        }
    }
}