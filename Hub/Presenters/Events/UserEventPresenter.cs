using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses.Events;

namespace Hub.Presenters.Events
{
    public class UserEventPresenter : IOutputPort<UserEventResponse>
    {
        public UserEventResponse Response { get; private set; }
        
        public void Handle(UserEventResponse response)
        {
            Response = response;
        }
    }
}