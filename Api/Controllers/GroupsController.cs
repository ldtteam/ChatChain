using System;
using System.Threading.Tasks;
using Api.Core.DTO.UseCaseRequests.Group;
using Api.Core.DTO.UseCaseResponses.Group;
using Api.Core.Interfaces.UseCases.Group;
using Api.Extensions;
using Api.Models.Request.Group;
using Api.Presenters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{organisation}/groups")]
    [Authorize]
    [ApiController]
    public class GroupsController : Controller
    {
        private readonly DefaultPresenter _defaultPresenter;
        private readonly IGetGroupsUseCase _getGroupsUseCase;
        private readonly ICreateGroupUseCase _createGroupUseCase;
        private readonly IGetGroupUseCase _getGroupUseCase;
        private readonly IUpdateGroupUseCase _updateGroupUseCase;
        private readonly IDeleteGroupUseCase _deleteGroupUseCase;

        public GroupsController(DefaultPresenter defaultPresenter, IGetGroupsUseCase getGroupsUseCase, ICreateGroupUseCase createGroupUseCase, IGetGroupUseCase getGroupUseCase, IUpdateGroupUseCase updateGroupUseCase, IDeleteGroupUseCase deleteGroupUseCase)
        {
            _defaultPresenter = defaultPresenter;
            _getGroupsUseCase = getGroupsUseCase;
            _createGroupUseCase = createGroupUseCase;
            _getGroupUseCase = getGroupUseCase;
            _updateGroupUseCase = updateGroupUseCase;
            _deleteGroupUseCase = deleteGroupUseCase;
        }
        
        [HttpGet("", Name = "GetGroups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetGroupsResponse>> GetGroupsAsync(Guid organisation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _getGroupsUseCase.HandleAsync(new GetGroupsRequest(User.GetId(), organisation), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpPost("", Name = "CreateGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CreateGroupResponse>> CreateGroupAsync(Guid organisation, CreateGroupDTO createGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _createGroupUseCase.HandleAsync(new CreateGroupRequest(User.GetId(), organisation, createGroupDTO.Name, createGroupDTO.Description, createGroupDTO.ClientIds), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
        
        [HttpGet("{group}", Name = "GetGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetGroupResponse>> GetGroupAsync(Guid organisation, Guid group)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _getGroupUseCase.HandleAsync(new GetGroupRequest(User.GetId(), organisation, group), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpPost("{group}", Name = "UpdateGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UpdateGroupResponse>> UpdateGroupAsync(Guid organisation, Guid group, UpdateGroupDTO updateGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _updateGroupUseCase.HandleAsync(new UpdateGroupRequest(User.GetId(), organisation, group, updateGroupDTO.Name, updateGroupDTO.Description, updateGroupDTO.ClientIds), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
        
        [HttpDelete("{group}", Name = "DeleteGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DeleteGroupResponse>> DeleteGroupAsync(Guid organisation, Guid group)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _deleteGroupUseCase.HandleAsync(new DeleteGroupRequest(User.GetId(), organisation, group), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
    }
}