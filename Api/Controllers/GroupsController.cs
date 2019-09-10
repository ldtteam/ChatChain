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
    [Route("api/{organisation}/groups")]
    [Authorize]
    [ApiController]
    public class GroupsController : Controller
    {
        
        private readonly GroupService _groupService;
        private readonly VerificationService _verificationService;

        public GroupsController(GroupService groupService, VerificationService verificationService)
        {
            _groupService = groupService;
            _verificationService = verificationService;
        }
        
        [HttpGet("", Name = "GetGroups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroupsAsync(Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanGetGroups(org);
            return !result.Value ? result.Result : Ok(await _groupService.GetFromOwnerIdAsync(org.Id));
        }
        
        [HttpPost("", Name = "CreateGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Guid>> CreateGroupAsync(Guid organisation, Group group)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanCreateGroup(org);
            if (!result.Value)
                return result.Result;

            group.Id = Guid.NewGuid();
            group.OwnerId = org.Id;
            group.ClientIds = new List<Guid>();
            await _groupService.CreateAsync(group);
            
            return Ok(group.Id);
        }
        
        [HttpGet("for-client/{client}", Name = "GetGroupsForClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroupsAsync(Guid organisation, Guid client)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Client apiClient = await _verificationService.VerifyClient(client);
            ActionResult<bool> result = User.CanGetGroups(org);
            return !result.Value ? result.Result : Ok(await _groupService.GetFromClientIdAsync(apiClient.Id));
        }
        
        [HttpGet("{group}", Name = "GetGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Group>> GetGroupAsync(Guid organisation, Guid group)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Group apiGroup = await _verificationService.VerifyGroup(group);
            ActionResult<bool> result = User.CanGetGroup(org, apiGroup);
            return !result.Value ? result.Result : Ok(apiGroup);
        }
        
        [HttpPost("{group}", Name = "UpdateGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> CreateGroupAsync(Guid organisation, Guid group, Group updateGroup)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Group apiGroup = await _verificationService.VerifyGroup(group);
            ActionResult<bool> result = User.CanUpdateGroup(org, apiGroup);
            if (!result.Value)
                return result.Result;

            updateGroup.Id = apiGroup.Id;
            updateGroup.OwnerId = org.Id;
            await _groupService.UpdateAsync(group, updateGroup);
            
            return Ok();
        }
        
        [HttpDelete("{group}", Name = "DeleteGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> CreateGroupAsync(Guid organisation, Guid group)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            Group apiGroup = await _verificationService.VerifyGroup(group);
            ActionResult<bool> result = User.CanDeleteGroup(org, apiGroup);
            if (!result.Value)
                return result.Result;

            await _groupService.RemoveAsync(apiGroup.Id);
            
            return Ok();
        }
    }
}