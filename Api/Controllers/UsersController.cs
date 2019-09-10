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
using ChatChainCommon.DatabaseServices;
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
        private readonly IdentityServerConnection _identityServerConnection;
        private readonly OrganisationService _organisationService;
        private readonly VerificationService _verificationService;

        public UsersController(IdentityServerConnection identityServerConnection, OrganisationService organisationService, VerificationService verificationService)
        {
            _identityServerConnection = identityServerConnection;
            _organisationService = organisationService;
            _verificationService = verificationService;
        }

        [HttpGet("", Name = "GetUsers")]
        public async Task<ActionResult<IDictionary<string, ResponseUser>>> GetUsersAsync(Guid organisation)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanGetUsers(org);

            return !result.Value ? result.Result : Ok(await GetResponseUsersAsync(org));
        }

        [HttpGet("{user}", Name = "GetUser")]
        public async Task<ActionResult<ResponseUser>> GetUserAsync(Guid organisation, string user)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ResponseUser responseUser = await GetResponseUserAsync(org, user);
            ActionResult<bool> result = User.CanGetUser(org, responseUser);
            return !result.Value ? result.Result : Ok(responseUser);
        }
        
        [HttpPost("{user}", Name = "UpdateUser")]
        public async Task<ActionResult> UpdateUserAsync(Guid organisation, string user, OrganisationUser orgUser)
        {
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanUpdateUser(org, user);
            if (!result.Value)
                return result.Result;

            org.Users[user] = orgUser;
            await _organisationService.UpdateAsync(org.Id, org);
            
            return Ok();
        }

        [HttpDelete("{user}", Name = "DeleteUser")]
        public async Task<ActionResult> DeleteUserAsync(Guid organisation, string user)
        {
            // If organisation is valid, and User has DeleteOrgUsers permission
            Organisation org = await _verificationService.VerifyOrganisation(organisation);
            ActionResult<bool> result = User.CanDeleteUser(org, user);
            if (!result.Value)
                return result.Result;

            org.Users.Remove(user);
            await _organisationService.UpdateAsync(org.Id, org);

            return Ok();
        }
        
        #region Not Action
        [NonAction]
        private async Task<IDictionary<string, ResponseUser>> GetResponseUsersAsync(Organisation org)
        {
            HttpClient httpClient = new HttpClient();
            IdentityServerClient client = new IdentityServerClient(_identityServerConnection.ServerUrl, httpClient);
            IEnumerable<UserDetails> identityDetails = await client.GetUserDetailsAsync(org.Users.Keys);
            
            Dictionary<string, ResponseUser> returnDictionary = new Dictionary<string, ResponseUser>();
            foreach ((string key, OrganisationUser value) in org.Users)
            {
                UserDetails userDetails = identityDetails.First(details => details.Id == key);
                returnDictionary.Add(key, new ResponseUser
                {
                    OrganisationUser = value,
                    DisplayName = userDetails.DisplayName,
                    EmailAddress = userDetails.EmailAddress,
                    Id = key
                });
            }

            return returnDictionary;
        }

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