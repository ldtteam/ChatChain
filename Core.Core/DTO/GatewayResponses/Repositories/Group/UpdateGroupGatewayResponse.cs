using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Group
{
    public class UpdateGroupGatewayResponse : BaseGatewayResponse
    {
        public Entities.Group Group { get; }

        public UpdateGroupGatewayResponse(Entities.Group group, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Group = group;
        }
    }
}