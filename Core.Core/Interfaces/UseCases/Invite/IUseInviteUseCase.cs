using Api.Core.DTO.UseCaseRequests.Invite;
using Api.Core.DTO.UseCaseResponses.Invite;

namespace Api.Core.Interfaces.UseCases.Invite
{
    public interface IUseInviteUseCase : IUseCaseRequestHandler<UseInviteRequest, UseInviteResponse>
    {
    }
}