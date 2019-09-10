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
    [Route("api/can/{respond200}/{organisation}/clients")]
    [Authorize]
    [ApiController]
    public class CanClientsController : Controller
    {
        private readonly VerificationService _verificationService;

        public CanClientsController(VerificationService verificationService)
        {
            _verificationService = verificationService;
        }
        
        [HttpGet("", Name = "CanGetClients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetClientsAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanGetClients(org);
            return respond200 ? Ok(result.Value) : result;
        }

        [HttpGet("create", Name = "CanCreateClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanCreateClientAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanCreateClient(org);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{client}/client", Name = "CanGetClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetClientAsync(bool respond200, Guid organisation, Guid client)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Client apiClient = await _verificationService.VerifyClient(client);
            ActionResult<bool> result = User.CanGetClient(org, apiClient);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{client}/update", Name = "CanUpdateClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanUpdateClientAsync(bool respond200, Guid organisation, Guid client)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Client apiClient = await _verificationService.VerifyClient(client);
            ActionResult<bool> result = User.CanUpdateClient(org, apiClient);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{client}/delete", Name = "CanDeleteClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanDeleteClientAsync(bool respond200, Guid organisation, Guid client)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Client apiClient = await _verificationService.VerifyClient(client);
            ActionResult<bool> result = User.CanDeleteClient(org, apiClient);
            return respond200 ? Ok(result.Value) : result;
        }
    }
}