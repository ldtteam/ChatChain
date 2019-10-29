using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses;

namespace Hub.Presenters
{
    public class GenericMessagePresenter : IOutputPort<GenericMessageResponse>
    {
        public GenericMessageResponse Response { get; set; }
        
        public void Handle(GenericMessageResponse response)
        {
            Response = response;
        }
    }
}