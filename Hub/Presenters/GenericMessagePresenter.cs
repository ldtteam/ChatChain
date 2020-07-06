using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses;

namespace Hub.Presenters
{
    public class GenericMessagePresenter : IOutputPort<GenericMessageResponse>
    {
        public GenericMessageResponse Response { get; private set; }
        
        public void Handle(GenericMessageResponse response)
        {
            Response = response;
        }
    }
}