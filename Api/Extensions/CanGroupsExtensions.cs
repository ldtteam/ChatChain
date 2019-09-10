using System.Security.Claims;
using Api.Constants;
using ChatChainCommon.DatabaseModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions
{
    public static class CanGroupsExtensions
    {
        #region Public Methods
        public static ActionResult<bool> CanGetGroups(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            return true;
        }
        
        public static ActionResult<bool> CanCreateGroup(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.CreateGroups))
                return StatusCode(403, ApiResponseConstants.NoPermission);
            
            return true;
        }
        
        public static ActionResult<bool> CanGetGroups(this ClaimsPrincipal user, Organisation org, Client client)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (client == null)
                return StatusCode(404, ApiResponseConstants.ClientDoesntExist);

            if (client.OwnerId != org.Id)
                return StatusCode(403, ApiResponseConstants.ClientNotInOrganisation);
            
            return true;
        }
        
        public static ActionResult<bool> CanGetGroup(this ClaimsPrincipal user, Organisation org, Group group)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (group == null)
                return StatusCode(404, ApiResponseConstants.GroupDoesntExist);

            if (group.OwnerId != org.Id)
                return StatusCode(403, ApiResponseConstants.GroupNotInOrganisation);
            
            return true;
        }
        
        public static ActionResult<bool> CanUpdateGroup(this ClaimsPrincipal user, Organisation org, Group group)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.EditGroups))
                return StatusCode(403, ApiResponseConstants.NoPermission);

            if (group == null)
                return StatusCode(404, ApiResponseConstants.GroupDoesntExist);

            if (group.OwnerId != org.Id)
                return StatusCode(403, ApiResponseConstants.GroupNotInOrganisation);
            
            return true;
        }
        
        public static ActionResult<bool> CanDeleteGroup(this ClaimsPrincipal user, Organisation org, Group group)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.DeleteGroups))
                return StatusCode(403, ApiResponseConstants.NoPermission);

            if (group == null)
                return StatusCode(404, ApiResponseConstants.GroupDoesntExist);

            if (group.OwnerId != org.Id)
                return StatusCode(403, ApiResponseConstants.GroupNotInOrganisation);
            
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