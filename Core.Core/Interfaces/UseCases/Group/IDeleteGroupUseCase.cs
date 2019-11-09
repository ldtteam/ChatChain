using Api.Core.DTO.UseCaseRequests.Group;
using Api.Core.DTO.UseCaseResponses.Group;

namespace Api.Core.Interfaces.UseCases.Group
{
    public interface IDeleteGroupUseCase : IUseCaseRequestHandler<DeleteGroupRequest, DeleteGroupResponse>
    {
    }
}