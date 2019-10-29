using System.Collections.Generic;

namespace Api.Core.Entities
{
    public class OrganisationUser
    {
        public string Id { get; set; }

        public IList<OrganisationPermissions> Permissions { get; set; } = new List<OrganisationPermissions>();
    }
}