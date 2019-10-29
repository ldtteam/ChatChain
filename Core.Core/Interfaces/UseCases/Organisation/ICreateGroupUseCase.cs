using Api.Core.DTO.UseCaseRequests.Organisation;
using Api.Core.DTO.UseCaseResponses.Organisation;

namespace Api.Core.Interfaces.UseCases.Organisation
{
    public interface
        ICreateOrganisationUseCase : IUseCaseRequestHandler<CreateOrganisationRequest, CreateOrganisationResponse>
    {
    }
}