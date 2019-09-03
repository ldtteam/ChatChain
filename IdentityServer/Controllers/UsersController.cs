using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IEnumerable<ResponseUser>> PostAsync([FromBody] IEnumerable<string> userIds)
        {
            IList<ResponseUser> returnList = new List<ResponseUser>();
            foreach (string userId in userIds)
            {
                returnList.Add(new ResponseUser
                {
                    DisplayName = (await _userManager.FindByIdAsync(userId))?.DisplayName,
                    Id = userId
                });
            }
            return returnList;
        }

        public class ResponseUser
        {
            public string DisplayName { get; set; }
            public string Id { get; set; }
        }
        
    }
}