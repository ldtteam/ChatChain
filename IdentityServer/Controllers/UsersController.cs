using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("", Name = "GetUserDetails")]
        public async Task<IEnumerable<UserDetails>> PostAsync([FromBody] IEnumerable<string> userIds)
        {
            IList<UserDetails> returnList = new List<UserDetails>();
            foreach (string userId in userIds)
            {
                returnList.Add(new UserDetails
                {
                    DisplayName = (await _userManager.FindByIdAsync(userId))?.DisplayName,
                    EmailAddress = (await _userManager.FindByIdAsync(userId))?.Email,
                    Id = userId
                });
            }
            return returnList;
        }

        public class UserDetails
        {
            public string DisplayName { get; set; }
            public string EmailAddress { get; set; }
            public string Id { get; set; }
        }
        
    }
}