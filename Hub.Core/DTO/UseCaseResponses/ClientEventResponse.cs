using System.Collections.Generic;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseResponses
{
    public class ClientEventResponse : UseCaseResponseMessage<ClientEventMessage>
    {
        public ClientEventResponse(ClientEventMessage response) : base(response)
        {
        }

        public ClientEventResponse(IList<ClientEventMessage> messages, ClientEventMessage response) : base(messages, response)
        {
        }
    }
}