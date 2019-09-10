using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Api;
using Api.Extensions;
using Api.Models;
using Api.Services;
using ChatChainCommon.Config;
using ChatChainCommon.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.CanControllers
{
    [Produces("application/json")]
    [Route("api/can/{respond200}/{organisation}/users")]
    [Authorize]
    [ApiController]
    public class CanUsersController : Controller
    {
        private readonly IdentityServerConnection _identityServerConnection;
        private readonly VerificationService _verificationService;

        public CanUsersController(IdentityServerConnection identityServerConnection, VerificationService verificationService)
        {
            _identityServerConnection = identityServerConnection;
            _verificationService = verificationService;
        }
        
        [HttpGet("", Name = "CanGetUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetUsersAsync(bool respond200, Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanGetUsers(org);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{user}", Name = "CanGetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanGetUserAsync(bool respond200, Guid organisation, string user)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ResponseUser responseUser = await GetResponseUserAsync(org, user);
            ActionResult<bool> result = User.CanGetUser(org, responseUser);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{user}/update", Name = "CanUpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanUpdateUserAsync(bool respond200, Guid organisation, string user)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanUpdateUser(org, user);
            return respond200 ? Ok(result.Value) : result;
        }
        
        [HttpGet("{user}/delete", Name = "CanDeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> CanDeleteUserAsync(bool respond200, Guid organisation, string user)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanDeleteUser(org, user);
            return respond200 ? Ok(result.Value) : result;
        }
        
        #region Not Actions
        [NonAction]
        private async Task<ResponseUser> GetResponseUserAsync(Organisation org, string user)
        {
            if (!org.Users.ContainsKey(user))
                return null;
            
            HttpClient httpClient = new HttpClient();
            IdentityServerClient client = new IdentityServerClient(_identityServerConnection.ServerUrl, httpClient);
            IEnumerable<UserDetails> identityDetails = await client.GetUserDetailsAsync(new[] { user });

            return new ResponseUser
            {
                OrganisationUser = org.Users[user],
                DisplayName = identityDetails.First().DisplayName,
                EmailAddress = identityDetails.First().EmailAddress,
                Id = user
            };
        }
        #endregion
    }
}