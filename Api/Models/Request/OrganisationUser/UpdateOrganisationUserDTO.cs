using System.Collections.Generic;
using Api.Core.Entities;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Api.Models.Request.OrganisationUser
{
    public class UpdateOrganisationUserDTO
    {
        public IList<OrganisationPermissions> Permissions { get; set; }
    }
}