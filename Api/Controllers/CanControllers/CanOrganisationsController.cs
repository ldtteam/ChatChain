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
    [Route("api/can/{respond200}/{organisation}")]
    [Authorize]
    [ApiController]
    public class CanOrganisationsController : Controller
    {
        private readonly VerificationService _verificationService;

        public CanOrganisationsController(VerificationService verificationService)
        {
            _verificationService = verificationService;
        }
        
        [HttpGet("", Name = "CanGetOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetOrganisationAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanGetOrganisation(org);
            return respond200 ? Ok(result.Value) : result;
        }

        [HttpGet("update", Name = "CanUpdateOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanUpdateOrganisationAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanUpdateOrganisation(org);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("delete", Name = "CanDeleteOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanDeleteOrganisationAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanDeleteOrganisation(org);
            return respond200 ? Ok(result.Value) : result;
        }

        [HttpGet("leave", Name = "CanLeaveOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanLeaveOrganisationAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanLeaveOrganisation(org);
            return respond200 ? Ok(result.Value) : result;
        }
    }
}