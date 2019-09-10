using System.Security.Claims;
using Api.Constants;
using ChatChainCommon.DatabaseModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions
{
    public static class CanClientsExtensions
    {
        
        #region Public Methods
        public static ActionResult<bool> CanGetClients(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            return true;
        }
        
        public static ActionResult<bool> CanGetClient(this ClaimsPrincipal user, Organisation org, Client client)
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
        
        public static ActionResult<bool> CanCreateClient(this ClaimsPrincipal user, Organisation org)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.CreateClients))
                return StatusCode(403, ApiResponseConstants.NoPermission);
            
            return true;
        }
        
        public static ActionResult<bool> CanUpdateClient(this ClaimsPrincipal user, Organisation org, Client client)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.EditClients))
                return StatusCode(403, ApiResponseConstants.NoPermission);
            
            if (client == null)
                return StatusCode(404, ApiResponseConstants.ClientDoesntExist);

            if (client.OwnerId != org.Id)
                return StatusCode(403, ApiResponseConstants.ClientNotInOrganisation);

            return true;
        }
        
        public static ActionResult<bool> CanDeleteClient(this ClaimsPrincipal user, Organisation org, Client client)
        {
            if (org == null)
                return StatusCode(404, ApiResponseConstants.OrganisationDoesntExist);

            if (!org.UserIsMember(user))
                return StatusCode(403, ApiResponseConstants.NotOrganisationMember);

            if (!org.UserHasPermission(user, OrganisationPermissions.DeleteClients))
                return StatusCode(403, ApiResponseConstants.NoPermission);
            
            if (client == null)
                return StatusCode(404, ApiResponseConstants.ClientDoesntExist);

            if (client.OwnerId != org.Id)
                return StatusCode(403, ApiResponseConstants.ClientNotInOrganisation);

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