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
    [Route("api/can/{respond200}/{organisation}/groups")]
    [Authorize]
    [ApiController]
    public class CanGroupsController : Controller
    {
        private readonly VerificationService _verificationService;

        public CanGroupsController(VerificationService verificationService)
        {
            _verificationService = verificationService;
        }
        
        [HttpGet("", Name = "CanGetGroups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetGroupsAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanGetGroups(org);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("create", Name = "CanCreateGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanCreateGroupAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanCreateGroup(org);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{group}", Name = "CanGetGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetGroupAsync(bool respond200, Guid organisation, Guid group)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Group apiGroup = await _verificationService.VerifyGroup(group);
            ActionResult<bool> result = User.CanGetGroup(org, apiGroup);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("for-client/{client}", Name = "CanGetGroupsForClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetGroupsAsync(bool respond200, Guid organisation, Guid client)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Client apiClient = await _verificationService.VerifyClient(client);
            ActionResult<bool> result = User.CanGetGroups(org, apiClient);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{group}/update", Name = "CanUpdateGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanUpdateGroupAsync(bool respond200, Guid organisation, Guid group)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Group apiGroup = await _verificationService.VerifyGroup(group);
            ActionResult<bool> result = User.CanUpdateGroup(org, apiGroup);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{group}/delete", Name = "CanDeleteGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanDeleteGroupAsync(bool respond200, Guid organisation, Guid group)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Group apiGroup = await _verificationService.VerifyGroup(group);
            ActionResult<bool> result = User.CanDeleteGroup(org, apiGroup);
            return respond200 ? Ok(result.Value) : result;
        }
    }
}