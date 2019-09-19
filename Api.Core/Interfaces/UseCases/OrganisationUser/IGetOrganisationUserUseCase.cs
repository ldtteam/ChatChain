using Api.Core.DTO.UseCaseRequests.OrganisationUser;
using Api.Core.DTO.UseCaseResponses.OrganisationUser;

namespace Api.Core.Interfaces.UseCases.OrganisationUser
{
    public interface
        IGetOrganisationUserUseCase : IUseCaseRequestHandler<GetOrganisationUserRequest, GetOrganisationUserResponse>
    {
    }
}