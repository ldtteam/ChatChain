using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Core.Entities
{
    public class Organisation
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }
    }

    public class OrganisationDetails : Organisation
    {
        public IEnumerable<OrganisationUser> Users { get; set; } = new List<OrganisationUser>();

        //Since this is a DTO object, we sometimes wish to only return the Base class, not the derived one. Hence, this method.
        public Organisation ToOrganisation()
        {
            return new Organisation
            {
                Id = Id,
                Name = Name,
                Owner = Owner
            };
        }

        public bool UserIsOwner(string userId)
        {
            return Owner != null && Owner.Equals(userId);
        }

        public bool UserIsMember(string userId)
        {
            return Users.Select(u => u.Id).Contains(userId);
        }

        public bool UserHasPermission(string userId, OrganisationPermissions permissions)
        {
            if (Users == null || !Users.Select(u => u.Id).Contains(userId))
                return false;

            if (Owner != null && Owner.Equals(userId))
                return true;

            OrganisationUser orgUser = Users.First(u => u.Id == userId);

            return orgUser.Permissions.Contains(OrganisationPermissions.All) ||
                   orgUser.Permissions.Contains(permissions);
        }
    }
}