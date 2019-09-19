using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.ClientConfig
{
    public class GetClientConfigGatewayResponse : BaseGatewayResponse
    {
        public Entities.ClientConfig ClientConfig { get; }

        public GetClientConfigGatewayResponse(Entities.ClientConfig clientConfig, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            ClientConfig = clientConfig;
        }
    }
}