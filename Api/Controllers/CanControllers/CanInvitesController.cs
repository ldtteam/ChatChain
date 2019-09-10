using System;
using System.Threading.Tasks;
using Api.Extensions;
using Api.Services;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.CanControllers
{
    [Produces("application/json")]
    [Route("api/can/{respond200}/{organisation}/invites")]
    [Authorize]
    [ApiController]
    public class CanInvitesController : Controller
    {
        private readonly OrganisationService _organisationService;
        private readonly VerificationService _verificationService;

        public CanInvitesController(OrganisationService organisationService, VerificationService verificationService)
        {
            _organisationService = organisationService;
            _verificationService = verificationService;
        }
        
        [HttpGet("", Name = "CanCreateInvite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CreateInviteAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanCreateInvite(org);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{token}", Name = "CanUseInvite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> UseInviteAsync(bool respond200, Guid organisation, string token)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            OrganisationInvite invite = await _organisationService.GetInviteAsync(org.Id, token);
            ActionResult<bool> result = User.CanUseInvite(org, invite);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{token}/organisation", Name = "CanGetOrganisationWithInvite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetOrganisationWithInviteAsync(bool respond200, Guid organisation, string token)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            OrganisationInvite invite = await _organisationService.GetInviteAsync(org.Id, token);
            ActionResult<bool> result = User.CanUseInvite(org, invite);
            return respond200 ? Ok(result.Value) : result;
        }
    }
}