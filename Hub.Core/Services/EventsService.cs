using System;
using Hub.Core.DTO.UseCaseRequests;

namespace Hub.Core.Services
{
    public sealed class EventsService
    {
        public event EventHandler<GenericMessageRequest> GenericMessageEvent;

        public void OnGenericMessageEvent(GenericMessageRequest request)
        {
            GenericMessageEvent?.Invoke(this, request);
        }
    }
}