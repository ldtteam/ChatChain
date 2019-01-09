using System;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatChainServer.Data;
using ChatChainServer.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;

namespace ChatChainServer.Utils
{
    internal interface IRabbitMqService
    {
        void DeleteEvent(string groupId);
        void AddClientEvent(string groupId);
        void RemoveClientEvent(string groupId, string clientId);
    }

    internal class ScopedRabbitMqService : IRabbitMqService
    {
        private readonly IHubContext<ChatChainHub> _hubContext;
        private readonly GroupsDbContext _groupsDbContext;

        public ScopedRabbitMqService(IHubContext<ChatChainHub> hubContext, GroupsDbContext groupsDbContext)
        {
            _hubContext = hubContext;
            _groupsDbContext = groupsDbContext;
            Console.WriteLine("Scoped Rabbit Mq Service Started");
        }
        
        public async void DeleteEvent(string groupId)
        {
            
            Console.WriteLine("DeleteEvent Ran");

            var group = await _groupsDbContext.Groups.Include(g => g.ClientGroups).ThenInclude(cg => cg.Client)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null) return;
            {
                foreach (var cg in group.ClientGroups.ToList())
                {
                    foreach (var connectionId in Startup.ClientIds[cg.Client.ClientGuid])
                    {
                        await _hubContext.Groups.RemoveFromGroupAsync(connectionId, group.GroupId);
                    }
                }
            }
        }

        public async void AddClientEvent(string groupId)
        {

            Console.WriteLine("AddClientEvent Ran");
            
            var group = await _groupsDbContext.Groups.Include(g => g.ClientGroups).ThenInclude(cg => cg.Client)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null) return;
            {
                foreach (var cg in group.ClientGroups)
                {
                    if (!Startup.ClientIds.ContainsKey(cg.Client.ClientGuid)) continue;
                    foreach (var connectionId in Startup.ClientIds[cg.Client.ClientGuid])
                    {
                        await _hubContext.Groups.AddToGroupAsync(connectionId, group.GroupId);
                    }
                }
            }
        }
        
        public async void RemoveClientEvent(string groupId, string clientId)
        {

            Console.WriteLine("RemoveClientEvent Ran");

            var group = await _groupsDbContext.Groups.Include(g => g.ClientGroups).ThenInclude(cg => cg.Client)
                .FirstOrDefaultAsync(g => g.GroupId == groupId);

            if (group == null) return;
            {
                if (!Startup.ClientIds.ContainsKey(clientId)) return;
                foreach (var connectionId in Startup.ClientIds[clientId])
                {
                    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, @group.GroupId);
                }
            }
        }
    }
    
    internal class RabbitMqService : IHostedService
    {

        private IServiceProvider Services { get; }
        private IConnection _connection { get; set; }

        public RabbitMqService(IServiceProvider services)
        {
            Services = services;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory {HostName = "66.70.180.4"};
            
            Console.WriteLine("test1235");

            _connection = factory.CreateConnection();
                
            using (var channel = _connection.CreateModel())
            {
                
                Console.WriteLine("test123456");

                channel.ExchangeDeclare("actions", ExchangeType.Fanout, true, false, null);
                channel.QueueDeclare("actions_queue", true, false, false, null);
                channel.QueueBind("actions_queue", "actions", "");

                /*var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                    exchange: "actions",
                    routingKey: "");*/
                
                Console.WriteLine("test1234567");
                
                var consumer = new EventingBasicConsumer(channel);
                
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var deserialized = JsonConvert.DeserializeObject<EventMessage>(message);
                    Console.WriteLine(" [x] Received [{0}], [{1}], [{2}] ", message, deserialized.EventName,
                        deserialized.GroupId);

                    using (var scope = Services.CreateScope())
                    {
                        var scopedRabbitMqService =
                            scope.ServiceProvider
                                .GetRequiredService<IRabbitMqService>();

                        switch (deserialized.EventName)
                        {
                            case EventMessage.DeleteEvent:
                                scopedRabbitMqService.DeleteEvent(deserialized.GroupId);
                                break;
                            case EventMessage.AddClientEvent:
                                scopedRabbitMqService.AddClientEvent(deserialized.GroupId);
                                break;
                            case EventMessage.RemoveClientEvent:
                                scopedRabbitMqService.RemoveClientEvent(deserialized.GroupId, deserialized.ClientId);
                                break;
                        }
                    }
                };
                
                channel.BasicConsume(queue: "actions_queue",
                    autoAck: true,
                    consumer: consumer);
                Console.WriteLine("test1234568");
            }

            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();
            
            return Task.CompletedTask;
        }
        
    }
    
    internal class EventMessage
    {
        public const string DeleteEvent = "group_delete";
        public const string AddClientEvent = "group_add_client";
        public const string RemoveClientEvent = "group_remove_client";
        public string EventName { get; set; }
        public string GroupId { get; set; }
        public string ClientId { get; set; }
    }
}