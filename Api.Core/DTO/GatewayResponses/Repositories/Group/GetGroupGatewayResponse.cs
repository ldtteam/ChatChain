using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Group
{
    public class GetGroupGatewayResponse : BaseGatewayResponse
    {
        public Entities.Group Group { get; }

        public GetGroupGatewayResponse(Entities.Group group, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Group = group;
        }
    }
}