using Hub.Core.Services;

namespace Hub.EventConsumers
{
    public class GenericMessageEventConsumer
    {
        public GenericMessageEventConsumer(EventsService eventsService)
        {
            eventsService.GenericMessageEvent += (sender, request) => request.Message = "testing";
        }
    }
}