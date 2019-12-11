using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses.Events;

namespace Hub.Presenters.Events
{
    public class ClientEventPresenter : IOutputPort<ClientEventResponse>
    {
        public ClientEventResponse Response { get; private set; }
        
        public void Handle(ClientEventResponse response)
        {
            Response = response;
        }
    }
}