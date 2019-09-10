using System;
using System.Collections.Generic;
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
    [Route("api/")]
    [Authorize]
    [ApiController]
    public class OrganisationsController : Controller
    {
        private readonly OrganisationService _organisationService;
        private readonly VerificationService _verificationService;

        public OrganisationsController(OrganisationService organisationService, VerificationService verificationService)
        {
            _organisationService = organisationService;
            _verificationService = verificationService;
        }

        [HttpGet("organisations", Name = "GetOrganisations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Organisation>>> GetOrganisationsAsync()
        {
            return Ok(await _organisationService.GetForUserAsync(User.GetId()));
        }
        
        [HttpPost("organisations", Name = "CreateOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> CreateOrganisationAsync(Organisation org)
        {
            org.Users = new Dictionary<string, OrganisationUser>
            {
                {
                    User.GetId(),
                    new OrganisationUser
                    {
                        Permissions = new List<OrganisationPermissions> { OrganisationPermissions.All }
                    }
                }
            };
            org.Owner = User.GetId();

            org.Id = Guid.NewGuid();
            await _organisationService.CreateAsync(org);
            return Ok(org.Id);
        }

        [HttpGet("{organisation}", Name = "GetOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Organisation>> GetOrganisationAsync(Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanGetOrganisation(org);
            return !result.Value ? result.Result : Ok(org);
        }

        [HttpPost("{organisation}", Name = "UpdateOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateOrganisationAsync(Guid organisation, Organisation org)
        {
            Organisation dbOrg =
                await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanUpdateOrganisation(dbOrg);
            if (!result.Value)
                return result.Result;

            // Checks that can only be added in the actual Post.
            if (dbOrg.Users != org.Users)
                return StatusCode(403, "Organisation user cannot be updated via this method");

            if (dbOrg.Owner != org.Owner)
                return StatusCode(403, "Organisation owner cannot be updated via this method");

            org.Id = dbOrg.Id;
            await _organisationService.UpdateAsync(dbOrg.Id, org);
            
            return Ok();
        }

        [HttpDelete("{organisation}", Name = "DeleteOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteOrganisationAsync(Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanDeleteOrganisation(org);
            if (!result.Value)
                return result.Result;

            await _organisationService.RemoveAsync(org.Id);
            
            return Ok();
        }

        [HttpGet("{organisation}/leave", Name = "LeaveOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> LeaveOrganisationAsync(Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanLeaveOrganisation(org);
            if (!result.Value)
                return result.Result;

            org.Users.Remove(User.GetId());
            await _organisationService.UpdateAsync(org.Id, org);
            
            return Ok();
        }
    }
}