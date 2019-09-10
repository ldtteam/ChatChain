using System;
using System.Security.Claims;
using Api.Constants;
using ChatChainCommon.DatabaseModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions
{
    public static class CanInviteExtensions
    {
        #region Public Methods
        public static ActionResult<bool> CanCreateInvite(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.CreateOrgUsers))
                return StatusCode(403, ApiResponseConstants.NoPermission);

            return true;
        }
        
        public static ActionResult<bool> CanUseInvite(this ClaimsPrincipal user, Organisation org, OrganisationInvite invite)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (org.UserIsMember(user))
                return StatusCode(403, "User is already organisation member");

            if (invite == null)
                return StatusCode(404, ApiResponseConstants.InviteDoesntExist);

            if (invite.OrganisationId != org.Id)
                return StatusCode(403, "Organisation and Invite Organisation do not match");
            
            if (!user.GetClaim("EmailAddress").Equals(invite.Email, StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, "User Email Address does not match invite");

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