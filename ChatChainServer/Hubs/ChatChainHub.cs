using System.Security.Claims;
using System.Threading.Tasks;
using ChatChainServer.Models;
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

            /*foreach (var group in _clientsContext.GetGroups(_clientsContext.GetFromClientGuid(Context.UserIdentifier).Id
                .ToString()))
            {
                _logger.LogInformation($" Adding Client {Context.UserIdentifier} to group {group.GroupId}");
                Groups.AddToGroupAsync(Context.ConnectionId, group.GroupId);
            }*/
            
            return base.OnConnectedAsync();
        }
        
        // ClientType is what ChatChain extension is connecting. E.G. "ChatChainDC", These should be Unique!
        // ClientName is the name of the specific client connecting. E.G. "Minecolonies Test Server", These should be Unique!d
        // Channel is used to specify a chat channel. E.G. "staff" channel.

        public class User
        {
            public string Name { get; set; }
        }
        
        public class GenericMessage
        {
            public string Channel { get; set; }
            public User User { get; set; }
            public string Message { get; set; }
            public Client SendingClient { get; set; }
            public bool SendToSelf { get; set; }
        }

        public async Task SendGenericMessage(GenericMessage message)
        {
            _logger.LogInformation($"Client of Name: {Context.UserIdentifier} had author: {message.User.Name} send \"{message.Message}\" in channel: {message.Channel}");
            _logger.LogInformation($"Client: {Context.ConnectionId}, User: {Context.UserIdentifier}");

            var group = _groupsContext.GetFromGuid(message.Channel);
            var client = _clientsContext.GetFromClientGuid(Context.UserIdentifier);
            
            if (group != null && client != null && group.ClientIds.Contains(client.Id))
            {
                message.SendingClient = client;
                _logger.LogInformation($"Client Id: {client.ClientId} SendToSelf: {message.SendToSelf}");
                foreach (var fClient in _groupsContext.GetClients(group.Id.ToString()))
                {
                    _logger.LogInformation($"fClient Name: {fClient.ClientName} fClient ID: {fClient.ClientId}");   
                    if (!fClient.ClientId.Equals(client.ClientId) || message.SendToSelf)
                    {
                        await Clients.User(fClient.ClientGuid).SendAsync("ReceiveGenericMessage", message);
                    }
                }
                
                /*message.SendingClient = _clientsContext.GetFromClientGuid(Context.UserIdentifier).ClientName;
                
                await Clients.Group(message.Channel).SendAsync("ReceiveGenericMessage", message);*/
            }
        }
    }
}