using System.Collections.Generic;

namespace Hub.Core.Interfaces
{
    public abstract class UseCaseResponseMessage<TMessageResponse> where TMessageResponse : MessageResponse
    {
        public IList<TMessageResponse> Messages { get; }
        
        public TMessageResponse Response { get; }

        public UseCaseResponseMessage(TMessageResponse response)
        {
            Response = response;
            Messages = new List<TMessageResponse>();
        }

        public UseCaseResponseMessage(IList<TMessageResponse> messages, TMessageResponse response)
        {
            Messages = messages;
            Response = response;
        }
    }
}