using System.Collections.Generic;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseResponses
{
    public class GetClientResponse : UseCaseResponseMessage<GetClientMessage>
    {
        public GetClientResponse(GetClientMessage response) : base(response)
        {
        }

        public GetClientResponse(IList<GetClientMessage> messages, GetClientMessage response) : base(messages, response)
        {
        }
    }
}