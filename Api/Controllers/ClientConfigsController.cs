using System;
using System.Threading.Tasks;
using Api.Core.DTO.UseCaseRequests.ClientConfig;
using Api.Core.DTO.UseCaseResponses.ClientConfig;
using Api.Core.Interfaces.UseCases.ClientConfig;
using Api.Extensions;
using Api.Models.Request.ClientConfig;
using Api.Presenters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/{organisation}/clients/configs")]
    [Authorize]
    [ApiController]
    public class ClientConfigsController : ControllerBase
    {
        private readonly DefaultPresenter _defaultPresenter;
        private readonly IGetClientConfigUseCase _getClientConfigUseCase;
        private readonly IUpdateClientConfigUseCase _updateClientConfigUseCase;

        public ClientConfigsController(DefaultPresenter defaultPresenter, IGetClientConfigUseCase clientConfigUseCase, IUpdateClientConfigUseCase updateClientConfigUseCase)
        {
            _defaultPresenter = defaultPresenter;
            _getClientConfigUseCase = clientConfigUseCase;
            _updateClientConfigUseCase = updateClientConfigUseCase;
        }

        [HttpGet("{config}", Name = "GetClientConfig")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetClientConfigResponse>> GetClientConfigAsync(Guid organisation, Guid config)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _getClientConfigUseCase.HandleAsync(new GetClientConfigRequest(User.GetId(), organisation, config),
                _defaultPresenter);

            return _defaultPresenter.ContentResult;
        }

        [HttpPost("{config}", Name = "UpdateClientConfig")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UpdateClientConfigResponse>> UpdateClientConfigAsync(Guid organisation, Guid config, UpdateClientConfigDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _updateClientConfigUseCase.HandleAsync(
                new UpdateClientConfigRequest(User.GetId(), organisation, config, request.ClientEventGroups, request.UserEventGroups),
                _defaultPresenter);
            
            return _defaultPresenter.ContentResult;
        }
    }
}