using System.Collections.Generic;
using Hub.Core.DTO.ResponseMessages.Stats;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseResponses.Stats
{
    public class StatsResponseResponse : UseCaseResponseMessage<StatsResponseMessage>
    {
        public StatsResponseResponse(StatsResponseMessage response) : base(response)
        {
        }

        public StatsResponseResponse(IList<StatsResponseMessage> messages, StatsResponseMessage response) : base(messages, response)
        {
        }
    }
}