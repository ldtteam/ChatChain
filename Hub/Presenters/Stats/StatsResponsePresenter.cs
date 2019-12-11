using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseResponses.Stats;

namespace Hub.Presenters.Stats
{
    public class StatsResponsePresenter : IOutputPort<StatsResponseResponse>
    {
        public StatsResponseResponse Response { get; private set; }
        
        public void Handle(StatsResponseResponse response)
        {
            Response = response;
        }
    }
}