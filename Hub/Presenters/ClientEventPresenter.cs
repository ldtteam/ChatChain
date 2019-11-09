using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses;

namespace Hub.Presenters
{
    public class ClientEventPresenter : IOutputPort<ClientEventResponse>
    {
        public ClientEventResponse Response { get; set; }
        
        public void Handle(ClientEventResponse response)
        {
            Response = response;
        }
    }
}