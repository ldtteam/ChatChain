using System;
using System.Threading.Tasks;
using ChatChainCommon.DatabaseModels;
using ChatChainCommon.DatabaseServices;

namespace Api.Services
{
    public class VerificationService
    {
        private readonly OrganisationService _organisationService;
        private readonly ClientService _clientService;
        private readonly ClientConfigService _clientConfigService;
        private readonly GroupService _groupService;

        public VerificationService(OrganisationService organisationService, ClientService clientService, ClientConfigService clientConfigService, GroupService groupService)
        {
            _organisationService = organisationService;
            _clientService = clientService;
            _clientConfigService = clientConfigService;
            _groupService = groupService;
        }

        public async Task<Organisation> VerifyOrganisation(Guid organisationId)
        {
            return await _organisationService.GetAsync(organisationId);
        }
        
        public async Task<Group> VerifyGroup(Guid groupId)
        {
            return await _groupService.GetAsync(groupId);
        }
        
        public async Task<Client> VerifyClient(Guid clientId)
        {
            return await _clientService.GetAsync(clientId);
        }
        
        public async Task<ClientConfig> VerifyClientConfig(Guid clientConfigId)
        {
            return await _clientConfigService.GetAsync(clientConfigId);
        }
    }
}