using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseRequests.Events;
using Hub.Core.DTO.UseCaseResponses.Events;

namespace Hub.Core.Interfaces.UseCases.Events
{
    public interface IUserEventUseCase : IUseCaseRequestHandler<UserEventRequest, UserEventResponse>
    {
    }
}