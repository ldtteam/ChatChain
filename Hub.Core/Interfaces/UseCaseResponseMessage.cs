using System.Collections.Generic;

namespace Hub.Core.Interfaces
{
    public abstract class UseCaseResponseMessage<TMessageResponse>
    {
        public IList<TMessageResponse> Messages { get; }
        
        public TMessageResponse Response { get; }

        protected UseCaseResponseMessage(TMessageResponse response)
        {
            Response = response;
            Messages = new List<TMessageResponse>();
        }

        protected UseCaseResponseMessage(IList<TMessageResponse> messages, TMessageResponse response)
        {
            Messages = messages;
            Response = response;
        }
    }
}