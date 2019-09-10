using System.Security.Claims;
using Api.Constants;
using ChatChainCommon.DatabaseModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions
{
    public static class CanClientConfigsExtensions
    {
        #region Public Methods
        public static ActionResult<bool> CanGetClientConfig(this ClaimsPrincipal user, Organisation org, ClientConfig clientConfig)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);
            
            if (clientConfig == null)
                return StatusCode(404, ApiResponseConstants.ClientConfigDoesntExist);

            if (clientConfig.OwnerId != org.Id)
                return StatusCode(403, ApiResponseConstants.ClientConfigNotInOrganisation);

            return true;
        }
        
        public static ActionResult<bool> CanUpdateClientConfig(this ClaimsPrincipal user, Organisation org, ClientConfig clientConfig)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.EditClients))
                return StatusCode(403, ApiResponseConstants.NoPermission);
            
            if (clientConfig == null)
                return StatusCode(404, ApiResponseConstants.ClientConfigDoesntExist);

            if (clientConfig.OwnerId != org.Id)
                return StatusCode(403, ApiResponseConstants.ClientConfigNotInOrganisation);

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