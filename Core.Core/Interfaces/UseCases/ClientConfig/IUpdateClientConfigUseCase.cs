using Api.Core.DTO.UseCaseRequests.ClientConfig;
using Api.Core.DTO.UseCaseResponses.ClientConfig;

namespace Api.Core.Interfaces.UseCases.ClientConfig
{
    public interface
        IUpdateClientConfigUseCase : IUseCaseRequestHandler<UpdateClientConfigRequest, UpdateClientConfigResponse>
    {
    }
}