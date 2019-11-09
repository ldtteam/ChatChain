using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;

namespace Api.Core.Interfaces.UseCases.OrganisationUser
{
    public interface
        IGetOrganisationUsersUseCase : IUseCaseRequestHandler<GetOrganisationUsersRequest, GetOrganisationUsersResponse>
    {
    }
}