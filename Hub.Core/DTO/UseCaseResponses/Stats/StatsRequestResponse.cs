using System.Collections.Generic;
using Hub.Core.DTO.ResponseMessages.Stats;
using Hub.Core.Interfaces;

namespace Hub.Core.DTO.UseCaseResponses.Stats
{
    public class StatsRequestResponse : UseCaseResponseMessage<StatsRequestMessage>
    {
        public StatsRequestResponse(StatsRequestMessage response) : base(response)
        {
        }

        public StatsRequestResponse(IList<StatsRequestMessage> messages, StatsRequestMessage response) : base(messages, response)
        {
        }
    }
}