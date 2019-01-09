using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatChainServer.Data
{
    public class GroupsDbContext : DbContext
    {
        public GroupsDbContext(DbContextOptions<GroupsDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Group> Groups { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientGroup>()
                .HasKey(cg => new {cg.ClientId, cg.GroupId});
            modelBuilder.Entity<ClientGroup>()
                .HasOne(cg => cg.Client)
                .WithMany(c => c.ClientGroups)
                .HasForeignKey(cg => cg.ClientId);
            modelBuilder.Entity<ClientGroup>()
                .HasOne(cg => cg.Group)
                .WithMany(g => g.ClientGroups)
                .HasForeignKey(cg => cg.GroupId);
        }
    }
    
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string OwnerId { get; set; }
        public string ClientGuid { get; set; }
        
        public List<ClientGroup> ClientGroups { get; set; }
    }
    
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string OwnerId { get; set; }

        public List<ClientGroup> ClientGroups { get; set; }
    }

    public class ClientGroup
    {
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}