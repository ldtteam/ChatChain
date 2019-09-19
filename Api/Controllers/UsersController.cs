using System;
using System.Threading.Tasks;
using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;
using Api.Core.Interfaces.UseCases.OrganisationUser;
using Api.Extensions;
using Api.Models.Request.OrganisationUser;
using Api.Presenters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{organisation}/users")]
    [Authorize]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly DefaultPresenter _defaultPresenter;
        private readonly IGetOrganisationUsersUseCase _getOrganisationUsersUseCase;
        private readonly IGetOrganisationUserUseCase _getOrganisationUserUseCase;
        private readonly IUpdateOrganisationUserUseCase _updateOrganisationUserUseCase;
        private readonly IDeleteOrganisationUserUseCase _deleteOrganisationUserUseCase;

        public UsersController(DefaultPresenter defaultPresenter, IGetOrganisationUsersUseCase getOrganisationUsersUseCase, IGetOrganisationUserUseCase getOrganisationUserUseCase, IUpdateOrganisationUserUseCase updateOrganisationUserUseCase, IDeleteOrganisationUserUseCase deleteOrganisationUserUseCase)
        {
            _defaultPresenter = defaultPresenter;
            _getOrganisationUsersUseCase = getOrganisationUsersUseCase;
            _getOrganisationUserUseCase = getOrganisationUserUseCase;
            _updateOrganisationUserUseCase = updateOrganisationUserUseCase;
            _deleteOrganisationUserUseCase = deleteOrganisationUserUseCase;
        }

        [HttpGet("", Name = "GetUsers")]
        public async Task<ActionResult<GetOrganisationUsersResponse>> GetUsersAsync(Guid organisation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _getOrganisationUsersUseCase.HandleAsync(new GetOrganisationUsersRequest(User.GetId(), organisation), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpGet("{user}", Name = "GetUser")]
        public async Task<ActionResult<GetOrganisationUserResponse>> GetUserAsync(Guid organisation, string user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _getOrganisationUserUseCase.HandleAsync(new GetOrganisationUserRequest(User.GetId(), organisation, user), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
        
        [HttpPost("{user}", Name = "UpdateUser")]
        public async Task<ActionResult<UpdateOrganisationUserResponse>> UpdateUserAsync(Guid organisation, string user, UpdateOrganisationUserDTO userUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _updateOrganisationUserUseCase.HandleAsync(new UpdateOrganisationUserRequest(User.GetId(), organisation, user, userUpdateDTO.Permissions), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpDelete("{user}", Name = "DeleteUser")]
        public async Task<ActionResult<DeleteOrganisationUserResponse>> DeleteUserAsync(Guid organisation, string user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _deleteOrganisationUserUseCase.HandleAsync(new DeleteOrganisationUserRequest(User.GetId(), organisation, user), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
    }
}