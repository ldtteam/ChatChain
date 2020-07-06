using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseRequests.Stats;
using Hub.Core.DTO.UseCaseResponses.Stats;

namespace Hub.Core.Interfaces.UseCases.Stats
{
    public interface IStatsRequestUseCase : IUseCaseRequestHandler<StatsRequestRequest, StatsRequestResponse>
    {
    }
}