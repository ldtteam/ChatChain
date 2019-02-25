using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatChainServer.Models.MessageObjects;
using ChatChainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatChainServer.Hubs
{

    [Authorize]
    public class ChatChainHub:Hub
    {

        private readonly ILogger<ChatChainHub> _logger;
        private readonly GroupService _groupsContext;
        private readonly ClientService _clientsContext;

        private bool _hasSentLeaveMessage = false;

        public ChatChainHub(ILogger<ChatChainHub> logger, GroupService groupsContext, ClientService clientsContext)
        {
            _logger = logger;
            _groupsContext = groupsContext;
            _clientsContext = clientsContext;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Connection: {Context.UserIdentifier}");
            foreach (Claim claim in Context.User.Claims)
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

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (!_hasSentLeaveMessage)
            {
                var message = new ClientEventMessage();
                message.Event = "STOP";
                message.SendToSelf = false;
                SendClientEventMessage(message).GetAwaiter();
                _logger.LogInformation("Sent Client Event STOP message");
            }
            
            return base.OnDisconnectedAsync(exception);
        }
        
        // ClientType is what ChatChain extension is connecting. E.G. "ChatChainDC", These should be Unique!
        // ClientName is the name of the specific client connecting. E.G. "Minecolonies Test Server", These should be Unique!d
        // Channel is used to specify a chat channel. E.G. "staff" channel.

        public async Task SendGenericMessage(GenericMessage message)
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} had author: {message.User.Name} send \"{message.Message}\" in channel: {message.Group.GroupId}");

            var group = _groupsContext.GetFromGuid(message.Group.GroupId);
            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);
            
            if (group != null && client != null && group.ClientIds.Contains(client.Id))
            {
                message.SendingClient = client;
                message.Group = group;
                foreach (var fClient in _groupsContext.GetClients(group.Id.ToString()))
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
            _logger.LogInformation($"Client {Context.UserIdentifier} sent event: {message.Event}");

            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (client != null)
            {
                if (message.Event.Equals("STOP"))
                {
                    _hasSentLeaveMessage = true;
                }
                message.SendingClient = client;
                foreach (var fClient in _clientsContext.GetFromOwnerId(client.OwnerId))
                {
                    if (!fClient.ClientId.Equals(client.ClientId) || message.SendToSelf)
                    {
                        _logger.LogInformation($"Sending {message.Event} to client: {fClient.ClientName}");
                        await Clients.User(fClient.ClientGuid).SendAsync("ReceiveClientEventMessage", message);
                    }
                }
            }
        }

        public async Task GetGroups()
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} requested their groups");
            
            var response = new GetGroupsResponseMessage();

            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (client != null)
            {
                response.Groups = _clientsContext.GetGroups(client.Id.ToString());
            }

            await Clients.Caller.SendAsync("ReceiveGroups", response);
        }
        
        public async Task GetClient()
        {
            _logger.LogInformation($"Client {Context.UserIdentifier} requested their Client");
            
            var response = new GetClientResponseMessage();

            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);

            if (client != null)
            {
                response.Client = client;
            }

            await Clients.Caller.SendAsync("ReceiveClient", response);
        }
        
    }
}