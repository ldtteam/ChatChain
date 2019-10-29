using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses;

namespace Hub.Presenters
{
    public class UserEventPresenter : IOutputPort<UserEventResponse>
    {
        public UserEventResponse Response { get; set; }
        
        public void Handle(UserEventResponse response)
        {
            Response = response;
        }
    }
}