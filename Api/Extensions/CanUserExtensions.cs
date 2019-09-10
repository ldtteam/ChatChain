using System;
using System.Linq;
using System.Security.Claims;
using Api.Constants;
using Api.Extensions;
using Api.Models;
using ChatChainCommon.DatabaseModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions
{
    public static class CanUserExtensions
    {
        #region Public Methods
        public static ActionResult<bool> CanGetUsers(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);
            
            return true;
        }
        
        public static ActionResult<bool> CanGetUser(this ClaimsPrincipal user, Organisation org, ResponseUser responseUser)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);
            
            if (responseUser == null)
                return StatusCode(404, ApiResponseConstants.UserDoesntExist);
            
            return true;
        }
        
        public static ActionResult<bool> CanUpdateUser(this ClaimsPrincipal user, Organisation org, string orgUserId)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.EditOrgUsers))
                return StatusCode(403, ApiResponseConstants.NoPermission);
            
            if (!org.Users.ContainsKey(orgUserId))
                return StatusCode(404, ApiResponseConstants.UserDoesntExist);

            if (orgUserId.Equals(org.Owner, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, "Organisation owners cannot be updated");
            
            string currentUserId = user.GetId();

            if (orgUserId.Equals(currentUserId, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, "Organisation users cannot update themselves");
            
            return true;
        }
        
        public static ActionResult<bool> CanDeleteUser(this ClaimsPrincipal user, Organisation org, string orgUserId)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.DeleteOrgUsers))
                return StatusCode(403, ApiResponseConstants.NoPermission);
            
            if (!org.Users.ContainsKey(orgUserId))
                return StatusCode(404, ApiResponseConstants.UserDoesntExist);

            if (orgUserId.Equals(org.Owner, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, "Organisation owners cannot be deleted");
            
            string currentUserId = user.GetId();
            if (orgUserId.Equals(currentUserId, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, "Organisation users cannot delete themselves");
            
            return true;
        }
        #endregion

        #region Private Methods
        private static ObjectResult StatusCode(int statusCode, object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = statusCode
            };
        }
        #endregion
    }
}