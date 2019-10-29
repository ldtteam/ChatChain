using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Group
{
    public class CreateGroupGatewayResponse : BaseGatewayResponse
    {
        public Entities.Group Group { get; }

        public CreateGroupGatewayResponse(Entities.Group group, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Group = group;
        }
    }
}