using System.Collections.Generic;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseResponses
{
    public class GenericMessageResponse : UseCaseResponseMessage<GenericMessageMessage>
    {
        public GenericMessageResponse(GenericMessageMessage response) : base(response)
        {
        }

        public GenericMessageResponse(IList<GenericMessageMessage> messages, GenericMessageMessage response) : base(messages, response)
        {
        }
    }
}