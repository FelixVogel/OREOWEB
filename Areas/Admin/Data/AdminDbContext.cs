using Vogel.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace Vogel.Areas.Admin.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
        {
        }

        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Session> Session { get; set; }
    }
}
