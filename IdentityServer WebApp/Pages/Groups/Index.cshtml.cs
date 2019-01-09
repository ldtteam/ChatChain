using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using IdentityServer_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class GroupsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GroupsDbContext _groupsContext;
        
        public GroupsModel(UserManager<IdentityUser> userManager, GroupsDbContext groupsContext, IConnectionFactory connectionFactory)
        {
            _userManager = userManager;
            _groupsContext = groupsContext;
            //Test(connectionFactory);
        }
        
        private void Test(IConnectionFactory factory)
        {

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                    routingKey: "task_queue",
                    basicProperties: properties,
                    body: body);
            }
        }

        public IList<Group> Groups { get; set; }

        public async Task OnGetAsync()
        {
            Groups = new List<Group>();

            foreach (var group in await _groupsContext.Groups.Include(g => g.ClientGroups).ThenInclude(cg => cg.Client).ToListAsync())
            {
                if (group.OwnerId == _userManager.GetUserAsync(User).Result.Id)
                {
                    Groups.Add(group);
                }
            }
        }
    }
}