using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses;

namespace Hub.Presenters
{
    public class GetClientPresenter: IOutputPort<GetClientResponse>
    {
        public GetClientResponse Response { get; private set; }
        
        public void Handle(GetClientResponse response)
        {
            Response = response;
        }
    }
}