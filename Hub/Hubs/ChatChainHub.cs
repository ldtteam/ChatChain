using System;
using System.Threading.Tasks;
using Hub.Core.DTO.ResponseMessages;
using Hub.Core.DTO.ResponseMessages.Events;
using Hub.Core.DTO.ResponseMessages.Stats;
using Hub.Core.DTO.UseCaseRequests;
using Hub.Core.DTO.UseCaseRequests.Events;
using Hub.Core.DTO.UseCaseRequests.Stats;
using Hub.Core.Interfaces.UseCases;
using Hub.Core.Interfaces.UseCases.Events;
using Hub.Core.Interfaces.UseCases.Stats;
using Hub.Presenters;
using Hub.Presenters.Events;
using Hub.Presenters.Stats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace Hub.Hubs
{
    [Authorize]
    public class ChatChainHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly ILogger<ChatChainHub> _logger;
        private readonly GenericMessagePresenter _genericMessagePresenter;
        private readonly IGenericMessageUseCase _genericMessageUseCase;
        
        private readonly ClientEventPresenter _clientEventPresenter;
        private readonly IClientEventUseCase _clientEventUseCase;
        private readonly UserEventPresenter _userEventPresenter;
        private readonly IUserEventUseCase _userEventUseCase;

        private readonly StatsRequestPresenter _statsRequestPresenter;
        private readonly IStatsRequestUseCase _statsRequestUseCase;
        private readonly StatsResponsePresenter _statsResponsePresenter;
        private readonly IStatsResponseUseCase _statsResponseUseCase;
        
        private readonly GetGroupsPresenter _getGroupsPresenter;
        private readonly IGetGroupsUseCase _getGroupsUseCase;
        private readonly GetClientPresenter _getClientPresenter;
        private readonly IGetClientUseCase _getClientUseCase;

        private bool _hasSentLeaveMessage;

        public ChatChainHub(ILogger<ChatChainHub> logger, GenericMessagePresenter genericMessagePresenter, IGenericMessageUseCase genericMessageUseCase, ClientEventPresenter clientEventPresenter, IClientEventUseCase clientEventUseCase, UserEventPresenter userEventPresenter, IUserEventUseCase userEventUseCase, StatsRequestPresenter statsRequestPresenter, IStatsRequestUseCase statsRequestUseCase, StatsResponsePresenter statsResponsePresenter, IStatsResponseUseCase statsResponseUseCase, GetGroupsPresenter groupsPresenter, IGetGroupsUseCase groupsUseCase, GetClientPresenter clientPresenter, IGetClientUseCase clientUseCase)
        {
            _logger = logger;
            _genericMessagePresenter = genericMessagePresenter;
            _genericMessageUseCase = genericMessageUseCase;
            _clientEventPresenter = clientEventPresenter;
            _clientEventUseCase = clientEventUseCase;
            _userEventPresenter = userEventPresenter;
            _userEventUseCase = userEventUseCase;
            _statsRequestPresenter = statsRequestPresenter;
            _statsRequestUseCase = statsRequestUseCase;
            _statsResponsePresenter = statsResponsePresenter;
            _statsResponseUseCase = statsResponseUseCase;
            _getGroupsPresenter = groupsPresenter;
            _getGroupsUseCase = groupsUseCase;
            _getClientPresenter = clientPresenter;
            _getClientUseCase = clientUseCase;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (!_hasSentLeaveMessage)
            {
                ClientEventRequest request = new ClientEventRequest("STOP", null, new Guid(Context.UserIdentifier));
                await SendClientEventMessage(request).ConfigureAwait(false);
                _logger.LogInformation("Sent Client Event STOP message");
            }

            await base.OnDisconnectedAsync(exception);
        }

        #region GenericMessage

        public async Task<GenericMessageMessage> SendGenericMessage(GenericMessageRequest request)
        {
            _logger.LogInformation(
                $"Client {Context.UserIdentifier} had author: {request.ClientUser.Name} send \"{request.Message}\" in channel: {request.GroupId}");

            request.ClientId = new Guid(Context.UserIdentifier);

            await _genericMessageUseCase.HandleAsync(request, _genericMessagePresenter);

            foreach (GenericMessageMessage genericMessageMessage in _genericMessagePresenter.Response.Messages)
            {
                await Clients.User(genericMessageMessage.ClientId.ToString())
                    .SendAsync("ReceiveGenericMessage", genericMessageMessage);
            }

            return _genericMessagePresenter.Response.Response;
        }
        
        #endregion

        #region Events

        // ReSharper disable once UnusedMethodReturnValue.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<ClientEventMessage> SendClientEventMessage(ClientEventRequest request)
        {  
            _logger.LogInformation($"Client {Context.UserIdentifier} sent event: {request.Event} with extra data: {request.EventData}");

            request.ClientId = new Guid(Context.UserIdentifier);

            if (request.Event.Equals("STOP"))
            {
                _hasSentLeaveMessage = true;
            }

            await _clientEventUseCase.HandleAsync(request, _clientEventPresenter);

            foreach (ClientEventMessage clientEventMessage in _clientEventPresenter.Response.Messages)
            {
                await Clients.User(clientEventMessage.ClientId.ToString())
                    .SendAsync("ReceiveClientEventMessage", clientEventMessage);
            }

            return _clientEventPresenter.Response.Response;
        }

        public async Task<UserEventMessage> SendUserEventMessage(UserEventRequest request)
        {
            _logger.LogInformation(
                $"Client {Context.UserIdentifier} with user: {request.ClientUser.Name} sent event: {request.Event} with extra data: {request.EventData}");

            request.ClientId = new Guid(Context.UserIdentifier);

            await _userEventUseCase.HandleAsync(request, _userEventPresenter);

            foreach (UserEventMessage userEventMessage in _userEventPresenter.Response.Messages)
            {
                await Clients.User(userEventMessage.ClientId.ToString())
                    .SendAsync("ReceiveUserEventMessage", userEventMessage);
            }

            return _userEventPresenter.Response.Response;
        }
        
        #endregion

        #region Stats

        public async Task<StatsRequestMessage> SendStatsRequest(StatsRequestRequest request)
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} requested stats from: {request.RequestedClient}");
            
            request.ClientId = new Guid(Context.UserIdentifier);

            await _statsRequestUseCase.HandleAsync(request, _statsRequestPresenter);
            
            foreach (StatsRequestMessage statsRequestMessage in _statsRequestPresenter.Response.Messages)
            {
                await Clients.User(statsRequestMessage.ClientId.ToString())
                    .SendAsync("ReceiveStatsRequest", statsRequestMessage);
            }

            return _statsRequestPresenter.Response.Response;
        }
        
        public async Task<StatsResponseMessage> SendStatsResponse(StatsResponseRequest request)
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} responded to stats request: {request.RequestId}");
            
            request.ClientId = new Guid(Context.UserIdentifier);

            await _statsResponseUseCase.HandleAsync(request, _statsResponsePresenter);
            
            foreach (StatsResponseMessage statsResponseMessage in _statsResponsePresenter.Response.Messages)
            {
                await Clients.User(statsResponseMessage.ClientId.ToString())
                    .SendAsync("ReceiveStatsResponse", statsResponseMessage);
            }

            return _statsResponsePresenter.Response.Response;
        }

        #endregion

        #region Getters

        public async Task<GetGroupsMessage> GetGroups()
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} requested their groups");

            await _getGroupsUseCase.HandleAsync(new GetGroupsRequest(new Guid(Context.UserIdentifier)),
                _getGroupsPresenter);

            return _getGroupsPresenter.Response.Response;
        }

        public async Task<GetClientMessage> GetClient()
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} requested their Client");

            await _getClientUseCase.HandleAsync(new GetClientRequest(new Guid(Context.UserIdentifier)),
                _getClientPresenter);

            return _getClientPresenter.Response.Response;
        }
        
        #endregion
    }
}