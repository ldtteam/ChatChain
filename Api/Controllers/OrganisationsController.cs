using System;
using System.Threading.Tasks;
using Api.Core.DTO.UseCaseRequests.Organisation;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.Organisation;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Interfaces.UseCases.Organisation;
using Api.Core.Interfaces.UseCases.OrganisationUser;
using Api.Extensions;
using Api.Models.Request.Organisation;
using Api.Presenters;
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
        private readonly DefaultPresenter _defaultPresenter;
        private readonly GetOrganisationPresenter _getOrganisationPresenter;
        private readonly IGetOrganisationsUseCase _getOrganisationsUseCase;
        private readonly ICreateOrganisationUseCase _createOrganisationUseCase;
        private readonly IGetOrganisationUseCase _getOrganisationUseCase;
        private readonly IUpdateOrganisationUseCase _updateOrganisationUseCase;
        private readonly IDeleteOrganisationUseCase _deleteOrganisationUseCase;
        private readonly IDeleteOrganisationUserUseCase _deleteOrganisationUserUseCase;

        public OrganisationsController(DefaultPresenter defaultPresenter, GetOrganisationPresenter getOrganisationPresenter, IGetOrganisationsUseCase getOrganisationsUseCase, ICreateOrganisationUseCase createOrganisationUseCase, IGetOrganisationUseCase getOrganisationUseCase, IUpdateOrganisationUseCase updateOrganisationUseCase, IDeleteOrganisationUseCase deleteOrganisationUseCase, IDeleteOrganisationUserUseCase deleteOrganisationUserUseCase)
        {
            _defaultPresenter = defaultPresenter;
            _getOrganisationPresenter = getOrganisationPresenter;
            _getOrganisationsUseCase = getOrganisationsUseCase;
            _createOrganisationUseCase = createOrganisationUseCase;
            _getOrganisationUseCase = getOrganisationUseCase;
            _updateOrganisationUseCase = updateOrganisationUseCase;
            _deleteOrganisationUseCase = deleteOrganisationUseCase;
            _deleteOrganisationUserUseCase = deleteOrganisationUserUseCase;
        }

        [HttpGet("organisations", Name = "GetOrganisations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetOrganisationsResponse>> GetOrganisationsAsync()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _getOrganisationsUseCase.HandleAsync(new GetOrganisationsRequest(User.GetId()), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
        
        [HttpPost("organisations", Name = "CreateOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateOrganisationResponse>> CreateOrganisationAsync(CreateOrganisationDTO orgDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _createOrganisationUseCase.HandleAsync(new CreateOrganisationRequest(User.GetId(), orgDTO.Name), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
        
        [HttpGet("{organisation}", Name = "GetOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetOrganisationResponse>> GetOrganisationAsync(Guid organisation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            //We don't provide the user ID because it:
            //Allows us to get the name and ID of an organisation without being a part of it. 
            //Used for displaying what organisation an invite belongs to usually.
            await _getOrganisationUseCase.HandleAsync(new GetOrganisationRequest(organisation), _getOrganisationPresenter);

            return _getOrganisationPresenter.ContentResult;
        }

        [HttpGet("{organisation}/details", Name = "GetOrganisationDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetOrganisationResponse>> GetOrganisationDetailsAsync(Guid organisation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _getOrganisationUseCase.HandleAsync(new GetOrganisationRequest(User.GetId(), organisation), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpPost("{organisation}", Name = "UpdateOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UpdateOrganisationResponse>> UpdateOrganisationAsync(Guid organisation, UpdateOrganisationDTO updateOrganisationDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _updateOrganisationUseCase.HandleAsync(new UpdateOrganisationRequest(User.GetId(), organisation, updateOrganisationDTO.Name), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpDelete("{organisation}", Name = "DeleteOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DeleteOrganisationResponse>> DeleteOrganisationAsync(Guid organisation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _deleteOrganisationUseCase.HandleAsync(new DeleteOrganisationRequest(User.GetId(), organisation), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpPost("{organisation}/leave", Name = "LeaveOrganisation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DeleteOrganisationUserResponse>> LeaveOrganisationAsync(Guid organisation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _deleteOrganisationUserUseCase.HandleAsync(new DeleteOrganisationUserRequest(User.GetId(), organisation, User.GetId()), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
    }
}