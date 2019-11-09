using System.Collections.Generic;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseResponses
{
    public class GetGroupsResponse : UseCaseResponseMessage<GetGroupsMessage>
    {
        public GetGroupsResponse(GetGroupsMessage response) : base(response)
        {
        }

        public GetGroupsResponse(IList<GetGroupsMessage> messages, GetGroupsMessage response) : base(messages, response)
        {
        }
    }
}