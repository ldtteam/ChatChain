using System;
using System.Threading.Tasks;
using Api.Core.DTO.UseCaseRequests.Invite;
using Api.Core.DTO.UseCaseResponses.Invite;
using Api.Core.Interfaces.UseCases.Invite;
using Api.Extensions;
using Api.Models.Request.Invite;
using Api.Presenters;
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
        private readonly DefaultPresenter _defaultPresenter;
        private readonly ICreateInviteUseCase _createInviteUseCase;
        private readonly IUseInviteUseCase _useInviteUseCase;

        public InvitesController(DefaultPresenter defaultPresenter, ICreateInviteUseCase createInviteUseCase, IUseInviteUseCase useInviteUseCase)
        {
            _defaultPresenter = defaultPresenter;
            _createInviteUseCase = createInviteUseCase;
            _useInviteUseCase = useInviteUseCase;
        }
        
        [HttpPost("", Name = "CreateInvite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CreateInviteResponse>> CreateInviteAsync(Guid organisation, CreateInviteDTO createDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _createInviteUseCase.HandleAsync(new CreateInviteRequest(User.GetId(), organisation, createDTO.EmailAddress), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpPost("{token}", Name = "UseInvite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UseInviteAsync(Guid organisation, string token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _useInviteUseCase.HandleAsync(new UseInviteRequest(User.GetId(),  organisation, User.GetClaim("EmailAddress"), token), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
    }
}