using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Group
{
    public class GetGroupsGatewayResponse : BaseGatewayResponse
    {
        public IEnumerable<Entities.Group> Groups { get; }

        public GetGroupsGatewayResponse(IEnumerable<Entities.Group> groups, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Groups = groups;
        }
    }
}