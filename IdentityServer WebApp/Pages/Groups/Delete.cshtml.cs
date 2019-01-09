using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Extensions;
using IdentityServer_WebApp.Data;
using IdentityServer_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Pages.Groups
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ConfigurationDbContext _is4Context;
        private readonly GroupsDbContext _groupsContext;
        private readonly IConnectionFactory _connectionFactory;

        public DeleteModel(UserManager<IdentityUser> userManager, ConfigurationDbContext is4Context, GroupsDbContext groupsContext, IConnectionFactory connectionFactory)
        {
            _userManager = userManager;
            _is4Context = is4Context;
            _groupsContext = groupsContext;
            _connectionFactory = connectionFactory;
        }
        
        [BindProperty]
        public Group Group { get; set; }
        public List<string> Clients { get; set; }
        public string ErrorMessage { get; private set; }
        
        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Group = await _groupsContext.Groups.Include(group => group.ClientGroups).ThenInclude(cg => cg.Client).FirstOrDefaultAsync(g => g.Id == id);
            
            if (Group == null || Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }

            Clients = new List<string>();

            foreach (var client in Group.ClientGroups.Select(cg => cg.Client))
            {
                var is4Client = await _is4Context.Clients.FirstAsync(c => c.Id == client.ClientId);
                Clients.Add(is4Client.ClientName);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete failed. Try again";
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }

            Group = await _groupsContext.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (Group.OwnerId != _userManager.GetUserAsync(User).Result.Id)
            {
                return RedirectToPage("./Index");
            }
            
            if (Group == null)
            {
                return RedirectToPage("./Index");
            }

            try
            {
                _groupsContext.Groups.Remove(Group);
                await _groupsContext.SaveChangesAsync();
                
                using(var connection = _connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    Console.WriteLine("test1234567");
                    channel.ExchangeDeclare("task_queue", "fanout");

                    var messageEnt = new EventMessage
                    {
                        EventName = EventMessage.DeleteEvent,
                        GroupId = Group.GroupId 
                    };
                    
                    var message = JsonConvert.SerializeObject(messageEnt);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "task_queue",
                        routingKey: "",
                        basicProperties: properties,
                        body: body);
                }

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction("./Delete",
                    new { id = id, saveChangesError = true });
            }
        }
    }
}