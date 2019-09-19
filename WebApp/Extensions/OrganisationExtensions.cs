using WebApp.Api;

namespace WebApp.Extensions
{
    public static class OrganisationExtensions
    {
        public static bool UserIsOwner(this OrganisationDetails organisation, OrganisationUser user)
        {
            return organisation.Owner.Equals(user.Id);
        }

        public static bool UserHasPermission(this OrganisationDetails organisation, OrganisationUser user,
            Permissions permission)
        {
            if (organisation.UserIsOwner(user))
                return true;

            return user.Permissions.Contains(Permissions.All) || user.Permissions.Contains(permission);
        }

        public static bool UserIsOwner(this Organisation organisation, OrganisationUser user)
        {
            return organisation.Owner.Equals(user.Id);
        }

        public static bool UserHasPermission(this Organisation organisation, OrganisationUser user,
            Permissions permission)
        {
            if (organisation.UserIsOwner(user))
                return true;

            return user.Permissions.Contains(Permissions.All) || user.Permissions.Contains(permission);
        }
    }
}