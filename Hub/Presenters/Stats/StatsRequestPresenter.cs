using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses.Stats;

namespace Hub.Presenters.Stats
{
    public class StatsRequestPresenter : IOutputPort<StatsRequestResponse>
    {
        public StatsRequestResponse Response { get; private set; }
        
        public void Handle(StatsRequestResponse response)
        {
            Response = response;
        }
    }
}