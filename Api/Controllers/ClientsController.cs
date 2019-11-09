using System;
using System.Threading.Tasks;
using Api.Core.DTO.UseCaseRequests.Client;
using Api.Core.DTO.UseCaseResponses.Client;
using Api.Core.Interfaces.UseCases.Client;
using Api.Extensions;
using Api.Models.Request.Client;
using Api.Presenters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{organisation}/clients")]
    [Authorize]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly DefaultPresenter _defaultPresenter;
        private readonly IGetClientsUseCase _getClientsUseCase;
        private readonly ICreateClientUseCase _createClientUseCase;
        private readonly IGetClientUseCase _getClientUseCase;
        private readonly IUpdateClientUseCase _updateClientUseCase;
        private readonly IDeleteClientUseCase _deleteClientUseCase;

        public ClientsController(DefaultPresenter defaultPresenter, IGetClientsUseCase getClientsUseCase, ICreateClientUseCase createClientUseCase, IGetClientUseCase getClientUseCase, IUpdateClientUseCase updateClientUseCase, IDeleteClientUseCase deleteClientUseCase)
        {
            _defaultPresenter = defaultPresenter;
            _getClientsUseCase = getClientsUseCase;
            _createClientUseCase = createClientUseCase;
            _getClientUseCase = getClientUseCase;
            _updateClientUseCase = updateClientUseCase;
            _deleteClientUseCase = deleteClientUseCase;
        }
        
        [HttpGet("", Name = "GetClients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetClientsResponse>> GetClientsAsync(Guid organisation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _getClientsUseCase.HandleAsync(new GetClientsRequest(User.GetId(), organisation), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpPost("", Name = "CreateClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CreateClientResponse>> CreateClientAsync(Guid organisation, CreateClientDTO createClientDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _createClientUseCase.HandleAsync(new CreateClientRequest(User.GetId(), organisation, createClientDTO.Name, createClientDTO.Description), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
        
        [HttpGet("{client}", Name = "GetClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<GetClientResponse>> GetClientAsync(Guid organisation, Guid client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _getClientUseCase.HandleAsync(new GetClientRequest(User.GetId(), organisation, client), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpPost("{client}", Name = "UpdateClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UpdateClientResponse>> UpdateClientAsync(Guid organisation, Guid client, UpdateClientDTO updateClientDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _updateClientUseCase.HandleAsync(new UpdateClientRequest(User.GetId(), organisation, client, updateClientDTO.Name, updateClientDTO.Description), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
        
        [HttpDelete("{client}", Name = "DeleteClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DeleteClientResponse>> DeleteClientAsync(Guid organisation, Guid client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _deleteClientUseCase.HandleAsync(new DeleteClientRequest(User.GetId(), organisation, client), _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }
    }
}