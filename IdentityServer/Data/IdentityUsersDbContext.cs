using IdentityServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Data
{
    public class IdentityUsersDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityUsersDbContext(DbContextOptions<IdentityUsersDbContext> options)
            : base(options)
        {
        }
    }
}
