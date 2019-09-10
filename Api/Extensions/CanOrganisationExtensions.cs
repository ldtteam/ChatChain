using System.Security.Claims;
using Api.Constants;
using ChatChainCommon.DatabaseModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions
{
    public static class CanOrganisationExtensions
    {

        #region Public Methods
        public static ActionResult<bool> CanGetOrganisation(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            return !org.UserIsMember(user) ? StatusCode(403, ApiResponseConstants.NotOrganisationMember) : true;
        }
        
        public static ActionResult<bool> CanUpdateOrganisation(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            return !org.UserHasPermission(user, OrganisationPermissions.EditOrg) ? StatusCode(403, ApiResponseConstants.NoPermission) : true;
        }
        
        public static ActionResult<bool> CanDeleteOrganisation(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            return !org.UserIsOwner(user) ? StatusCode(403, ApiResponseConstants.NotOrganisationOwner) : true;
        }
        
        public static ActionResult<bool> CanLeaveOrganisation(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            return org.UserIsOwner(user) ? StatusCode(403, "Organisation owner cannot leave via this method") : true;
        }
        #endregion

        #region Private Methods
        private static ActionResult<bool> StatusCode(int statusCode, object value)
        {
            ActionResult<bool> returnValue = new ObjectResult(value)
            {
                StatusCode = statusCode
            };
            return returnValue;
        }
        #endregion
    }
}