using System;
using System.Threading.Tasks;
using Api.Extensions;
using Api.Services;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;
using ChatChainCommon.RandomGenerator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{organisation}/invites")]
    [Authorize]
    [ApiController]
    public class InvitesController : Controller
    {
        private readonly OrganisationService _organisationService;
        private readonly VerificationService _verificationService;

        public InvitesController(OrganisationService organisationService, VerificationService verificationService)
        {
            _organisationService = organisationService;
            _verificationService = verificationService;
        }
        
        [HttpPost("", Name = "CreateInvite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<string>> CreateInviteAsync(Guid organisation, string emailAddress)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanCreateInvite(org);

            if (!result.Value)
                return result.Result;

            string token = PasswordGenerator.Generate();
            
            OrganisationInvite invite = new OrganisationInvite
            {
                Email = emailAddress,
                OrganisationId = org.Id,
                Token = token
            };

            await _organisationService.CreateInviteAsync(invite);
            
            return Ok(token);
        }

        [HttpPost("{token}", Name = "UseInvite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<string>> UseInviteAsync(Guid organisation, string token)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            OrganisationInvite invite = await _organisationService.GetInviteAsync(org.Id, token);
            ActionResult<bool> result = User.CanUseInvite(org, invite);
            if (!result.Value)
                return result.Result;

            org.Users.Add(User.GetId(), new OrganisationUser());
            await _organisationService.UpdateAsync(org.Id, org);
            await _organisationService.RemoveInviteAsync(org.Id, invite.Token);
            
            return Ok();
        }
        
        [HttpGet("{token}/organisation", Name = "GetOrganisationWithInvite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Organisation>> GetOrganisationWithInviteAsync(Guid organisation, string token)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            OrganisationInvite invite = await _organisationService.GetInviteAsync(org.Id, token);
            ActionResult<bool> result = User.CanUseInvite(org, invite);
            return !result.Value ? result.Result : Ok(org);
        }
    }
}