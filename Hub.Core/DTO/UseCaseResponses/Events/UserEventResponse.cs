using System.Collections.Generic;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.DTO.ResponseMessages.Events;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseResponses.Events
{
    public class UserEventResponse : UseCaseResponseMessage<UserEventMessage>
    {
        public UserEventResponse(UserEventMessage response) : base(response)
        {
        }

        public UserEventResponse(IList<UserEventMessage> messages, UserEventMessage response) : base(messages, response)
        {
        }
    }
}