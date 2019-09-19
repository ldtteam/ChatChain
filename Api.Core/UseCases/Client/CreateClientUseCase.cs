using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.DTO;
using Api.Core.DTO.GatewayResponses.Repositories.Client;
using Api.Core.DTO.GatewayResponses.Repositories.ClientConfig;
using Api.Core.DTO.GatewayResponses.Repositories.IS4Client;
using Api.Core.DTO.GatewayResponses.Repositories.Organisation;
using Api.Core.DTO.UseCaseRequests.Client;
using Api.Core.DTO.UseCaseResponses.Client;
using Api.Core.Entities;
using Api.Core.Interfaces;
using Api.Core.Interfaces.Gateways.Repositories;
using Api.Core.Interfaces.Services;
using Api.Core.Interfaces.UseCases.Client;

namespace Api.Core.UseCases.Client
{
    public class CreateClientUseCase : ICreateClientUseCase
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IClientConfigRepository _clientConfigRepository;
        private readonly IIS4ClientRepository _is4ClientRepository;
        private readonly IPasswordGenerator _passwordGenerator;

        public CreateClientUseCase(IOrganisationRepository organisationRepository, IClientRepository clientRepository, IClientConfigRepository clientConfigRepository, IIS4ClientRepository is4ClientRepository, IPasswordGenerator passwordGenerator)
        {
            _organisationRepository = organisationRepository;
            _clientRepository = clientRepository;
            _clientConfigRepository = clientConfigRepository;
            _is4ClientRepository = is4ClientRepository;
            _passwordGenerator = passwordGenerator;
        }

        public async Task<bool> HandleAsync(CreateClientRequest message, IOutputPort<CreateClientResponse> outputPort)
        {
            GetOrganisationGatewayResponse organisationGatewayResponse =
                await _organisationRepository.Get(message.OrganisationId);

            if (!organisationGatewayResponse.Success)
            {
                outputPort.Handle(
                    new CreateClientResponse(organisationGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.OrganisationUser organisationUser = null;

            if (message.UserId != null)
            {
                if (!organisationGatewayResponse.Organisation.UserHasPermission(message.UserId,
                    OrganisationPermissions.CreateClients))
                {
                    outputPort.Handle(new CreateClientResponse(new[] {new Error("404", "Organisation Not Found")},
                        message.UserId != null));
                    return false;
                }

                organisationUser =
                    organisationGatewayResponse.Organisation.Users.First(u => u.Id == message.UserId);
            }
            
            Guid clientId = Guid.NewGuid();
            string password = _passwordGenerator.Generate();
            
            IS4Client is4Client = new IS4Client
            {
                Id = clientId,
                OwnerId = organisationGatewayResponse.Organisation.Id,
                AllowedGrantTypes = new[] {"client_credentials"},
                AllowedScopes = new[] {"ChatChain"},
                AllowOfflineAccess = true
            };

            CreateIS4ClientGatewayResponse is4ClientGatewayResponse = await _is4ClientRepository.Create(is4Client, password);

            if (!is4ClientGatewayResponse.Success)
            {
                outputPort.Handle(new CreateClientResponse(is4ClientGatewayResponse.Errors, message.UserId != null));
                return false;
            }
            
            Entities.Client client = new Entities.Client
            {
                Id = clientId,
                OwnerId = organisationGatewayResponse.Organisation.Id,
                Name = message.Name,
                Description = message.Description
            };

            CreateClientGatewayResponse clientGatewayResponse = await _clientRepository.Create(client);

            if (!clientGatewayResponse.Success)
            {
                outputPort.Handle(new CreateClientResponse(clientGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            Entities.ClientConfig clientConfig = new Entities.ClientConfig
            {
                Id = clientId,
                OwnerId = organisationGatewayResponse.Organisation.Id
            };

            CreateClientConfigGatewayResponse clientConfigGatewayResponse =
                await _clientConfigRepository.Create(clientConfig);

            if (!clientConfigGatewayResponse.Success)
            {
                outputPort.Handle(new CreateClientResponse(clientConfigGatewayResponse.Errors, message.UserId != null));
                return false;
            }

            outputPort.Handle(new CreateClientResponse(clientGatewayResponse.Client,
                clientConfigGatewayResponse.ClientConfig, password,
                organisationGatewayResponse.Organisation.ToOrganisation(),
                organisationUser, message.UserId != null, true));
            return true;
        }
    }
}