using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace IdentityServer_WebApp.Data
{
    public class GroupsDbContext : DbContext
    {
        public GroupsDbContext(DbContextOptions<GroupsDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Group> Groups { get; set; }
        public DbSet<Client> Clients { get; set; }
    }

    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public int OwnerId { get; set; }

        public List<Client> Clients { get; set; }
    }

    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int OwnerId { get; set; }
    }
}