using System;
using System.Threading.Tasks;
using Api.Core.DTO.GatewayResponses;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.Entities;

namespace Api.Core.Interfaces.Gateways.Repositories
{
    public interface IOrganisationRepository
    {
        Task<GetOrganisationsGatewayResponse> GetForUser(string userId);

        Task<GetOrganisationGatewayResponse> Get(Guid id);

        Task<CreateOrganisationGatewayResponse> Create(OrganisationDetails organisation);

        Task<UpdateOrganisationGatewayResponse> Update(Guid organisationId, OrganisationDetails organisation);

        Task<BaseGatewayResponse> Delete(Guid id);
    }
}