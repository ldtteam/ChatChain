using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.IS4Client
{
    public class CreateIS4ClientGatewayResponse : BaseGatewayResponse
    {
        public CreateIS4ClientGatewayResponse(bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
        }
    }
}