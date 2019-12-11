using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses;

namespace Hub.Presenters
{
    public class GetGroupsPresenter: IOutputPort<GetGroupsResponse>
    {
        public GetGroupsResponse Response { get; private set; }
        
        public void Handle(GetGroupsResponse response)
        {
            Response = response;
        }
    }
}