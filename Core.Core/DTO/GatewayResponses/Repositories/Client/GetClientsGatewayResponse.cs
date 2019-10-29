using System.Collections.Generic;

namespace Api.Core.DTO.GatewayResponses.Repositories.Client
{
    public class GetClientsGatewayResponse : BaseGatewayResponse
    {
        public IEnumerable<Entities.Client> Clients { get; }

        public GetClientsGatewayResponse(IEnumerable<Entities.Client> clients, bool success = false,
            IEnumerable<Error> errors = null) : base(success, errors)
        {
            Clients = clients;
        }
    }
}