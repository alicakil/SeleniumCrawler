using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using OpenQA.Selenium.DevTools;
using System.Diagnostics.Metrics;
using System.Security.Principal;
using System;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Dashboard.Models
{
    public class Context : DbContext
    {
        // private readonly IConfiguration configuration;
        //public Context(IConfiguration configuration)
        public Context()
        {
            //  this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            optionsBuilder.UseNpgsql(AppConstants.Db.ConnectionString);
        }


        public DbSet<Log> Logs { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
    }
}
