using Api.Core.Interfaces;
using Hub.Core.DTO.UseCaseRequests;
using Hub.Core.DTO.UseCaseResponses;

namespace Hub.Core.Interfaces.UseCases
{
    public interface IGetGroupsUseCase : IUseCaseRequestHandler<GetGroupsRequest, GetGroupsResponse>
    {
        
    }
}