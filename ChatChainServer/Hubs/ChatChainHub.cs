using System;
using System.Linq;
using System.Threading.Tasks;
using ChatChainServer.Models;
using ChatChainServer.Models.MessageObjects;
using ChatChainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace ChatChainServer.Hubs
{

    [Authorize]
    public class ChatChainHub:Hub
    {
        private readonly ILogger<ChatChainHub> _logger;
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;
        private readonly CommandRegistryService _commandRegistry;
        private readonly CommandExecutionService _executionService;

        private bool _hasSentLeaveMessage;

        public ChatChainHub(ILogger<ChatChainHub> logger, GroupService groupsContext, ClientService clientsContext, CommandRegistryService commandRegistry, CommandExecutionService executionService)
        {
            _logger = logger;
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
            _commandRegistry = commandRegistry;
            _executionService = executionService;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Connection: {Context.UserIdentifier}");
            foreach (var claim in Context.User.Claims)
            {
                _logger.LogInformation($"Claim: {claim.Properties}, {claim.Issuer}, {claim.Value}");
            }

            foreach (var identity in Context.User.Identities )
            {
                _logger.LogInformation($"identity: {identity.Name}");
            }
            _logger.LogInformation($"Claims: {Context.User.Claims}");
            
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (!_hasSentLeaveMessage)
            {
                var message = new ClientEventMessage {Event = ClientEventType.STOP, SendToSelf = false};
                await SendClientEventMessage(message);
                _logger.LogInformation("Sent Client Event STOP message");
            }
            
            _commandRegistry.RemoveForClientGuid(Context.UserIdentifier);
            
            await base.OnDisconnectedAsync(exception);
        }

        
        // --- Send Methods --- \\
        public async Task SendGenericMessage(GenericMessage message)
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} had author: {message.ClientUser.Name} send \"{message.Message}\" in channel: {message.Group.GroupId}");

            var group = _groupsContext.GetFromGuid(message.Group.GroupId);
            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);
            
            if (group != null && client != null && group.ClientIds.Contains(client.Id))
            {
                message.SendingClient = client;
                message.Group = group;
                foreach (var fClient in _groupsContext.GetClients(group.Id))
                {
                    if (!fClient.ClientId.Equals(client.ClientId) || message.SendToSelf)
                    {
                        await Clients.User(fClient.ClientGuid).SendAsync("ReceiveGenericMessage", message);
                    }
                }
            }
        }

        public async Task SendClientEventMessage(ClientEventMessage message)
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} sent event: {message.Event} with extra data: {message.ExtraEventData}");

            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (client != null)
            {
                if (message.Event.Equals("STOP"))
                {
                    _hasSentLeaveMessage = true;
                }

                message.SendingClient = client;
                if (_clientsContext.GetClientConfig(client.Id) != null)
                {
                    foreach (var fGroupId in _clientsContext.GetClientConfig(client.Id).ClientEventGroups)
                    {
                        var group = _groupsContext.Get(fGroupId);
                        
                        if (group == null) continue;
                        message.Group = group;
                        foreach (var fClient in _groupsContext.GetClients(group.Id))
                        {
                            if (!fClient.ClientId.Equals(client.ClientId) || message.SendToSelf)
                            {
                                await Clients.User(fClient.ClientGuid)
                                    .SendAsync("ReceiveClientEventMessage", message);
                            }
                        }
                    }
                }
            }
        }

        public async Task SendUserEventMessage(UserEventMessage message)
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} with user: {message.User.Name} sent event: {message.Event} with extra data: {message.ExtraEventData}");

            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (client != null)
            {
                message.SendingClient = client;

                if (_clientsContext.GetClientConfig(client.Id) != null)
                {
                    foreach (var fGroupId in _clientsContext.GetClientConfig(client.Id).UserEventGroups)
                    {
                        var group = _groupsContext.Get(fGroupId);
                        
                        if (group == null) continue;
                        message.Group = group;
                        foreach (var fClient in _groupsContext.GetClients(group.Id))
                        {
                            if (!fClient.ClientId.Equals(client.ClientId) || message.SendToSelf)
                            {
                                await Clients.User(fClient.ClientGuid)
                                    .SendAsync("ReceiveUserEventMessage", message);
                            }
                        }
                    }
                }
            }
        }

        public async Task<CommandExecutionMessage> SendCommandExecutionMessage(CommandExecutionMessage executionMessage)
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} has executed command {executionMessage.Command.Id} for client {executionMessage.Command.Client.ClientGuid}");

            var sendingClient = _clientsContext.GetFromClientGuid(Context.UserIdentifier);
            var command = _commandRegistry.Get(executionMessage.Command.Id);
            if (sendingClient == null || command == null || sendingClient.OwnerId != command.OwnerId)
            {
                _logger.LogInformation($"{sendingClient == null} || {command == null} || {sendingClient?.OwnerId != command?.OwnerId}");
                return null;
            }
            var receivingClient = _clientsContext.GetFromClientGuid(command.Client.ClientGuid);
            if (receivingClient == null || receivingClient.OwnerId != sendingClient.OwnerId)
            {
                _logger.LogInformation($"{receivingClient == null} || {receivingClient?.OwnerId != sendingClient.OwnerId}");
                return null;
            }
            // we do this so that all variables are correct according to our database. NOT according to the sending clients.
            command.CommandArguments = executionMessage.Command.CommandArguments; 
            executionMessage.Command = command;
            executionMessage.SendingClient = sendingClient;
            await _executionService.CreateAsync(executionMessage);

            await Clients.User(receivingClient.ClientGuid)
                .SendAsync("ReceiveCommandExecutionMessage", executionMessage);

            return executionMessage;
        }

        public async Task SendCommandResponseMessage(CommandResponseMessage responseMessage)
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} has responded to a command execution by the id of: {responseMessage.Id} with {responseMessage.Response}");

            var sendingClient = _clientsContext.GetFromClientGuid(Context.UserIdentifier);
            var commandExecution = _executionService.Get(responseMessage.Id);
            if (sendingClient != null && commandExecution?.SendingClient != null)
            {
                var receivingClient = _clientsContext.GetFromClientGuid(commandExecution.SendingClient.ClientGuid);
                if (receivingClient != null)
                {
                    responseMessage.SendingClient = receivingClient;
                    responseMessage.SendingGroup = commandExecution.SendingGroup;

                    await Clients.User(receivingClient.ClientGuid)
                        .SendAsync("ReceiveCommandResponseMessage", responseMessage);

                    await _executionService.RemoveAsync(commandExecution);
                }
            }
        }
        
        // --- Register Methods --- \\
        
        public async Task RegisterCommand(CommandRegisterMessage commandRegisterMessage)
        {
            _logger.LogInformation(
                $"Client {Context.UserIdentifier} has registered  {commandRegisterMessage.Commands.Count} commands");

            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (client != null)
            {
                foreach (var command in commandRegisterMessage.Commands)
                {
                    command.Client = client;
                    command.OwnerId = client.OwnerId;
                
                    await _commandRegistry.CreateAsync(command);
                }
                
                var message = new GetCommandsResponse
                {
                    Commands = _commandRegistry.GetFromOwnerId(client.OwnerId).ToList()
                };

                foreach (var fClient in _clientsContext.GetFromOwnerId(client.OwnerId))
                {
                    await Clients.User(fClient.ClientGuid).SendAsync("ReceiveCommands", message);
                }
            }
        }
        
        // --- Get Methods --- \\

        public async Task<GetCommandsResponse> GetCommands()
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} requested the current commands");
            
            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            var message = new GetCommandsResponse();

            if (client != null)
            {
                message.Commands = _commandRegistry.GetFromOwnerId(client.OwnerId).ToList();
            }

            return message;
        }
        
        public async Task<GetGroupsResponse> GetGroups()
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} requested their groups");
            
            var response = new GetGroupsResponse();

            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (client != null)
            {
                response.Groups = _clientsContext.GetGroups(client.Id);
            }

            return response;
        }
        
        public async Task<GetClientResponse> GetClient()
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} requested their Client");
            
            var response = new GetClientResponse();

            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (client != null)
            {
                response.Client = client;
            }

            return response;
        }
        
    }
}
