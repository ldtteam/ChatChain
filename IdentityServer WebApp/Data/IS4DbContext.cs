using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer_WebApp.Data
{
    public class IS4DbContext : ConfigurationDbContext
    {
        public IS4DbContext(DbContextOptions<ConfigurationDbContext> options)
            : base(options, storeOptions: null)
        {
        }
    }
}