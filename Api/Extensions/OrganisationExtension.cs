using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ChatChainCommon.DatabaseModels;

namespace Api.Extensions
{
    public static class OrganisationExtension
    {
        public static bool UserIsMember(this Organisation organisation, ClaimsPrincipal user)
        {
            return organisation.Users.ContainsKey(user.Claims.First(claim => claim.Type.Equals("sub")).Value);
        }

        public static bool UserIsOwner(this Organisation organisation, ClaimsPrincipal user)
        {
            // Should always be verified separately to get appropriate API response.
            if (!organisation.UserIsMember(user))
                throw new InvalidOperationException();
            
            return organisation.Owner.Equals(user.Claims.First(claim => claim.Type.Equals("sub")).Value);
        }

        public static bool UserHasPermission(this Organisation organisation, ClaimsPrincipal user,
            OrganisationPermissions permission)
        {
            // Should always be verified separately to get appropriate API response.
            if (!organisation.UserIsMember(user))
                throw new InvalidOperationException();
            
            IList<OrganisationPermissions> permissions = organisation.Users[user.Claims.First(claim => claim.Type.Equals("sub")).Value].Permissions;
            return permissions.Contains(OrganisationPermissions.All) || permissions.Contains(permission);
        }
    }
}