using Dashboard.Models;
using Dashboard.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;

namespace Dashboard.Helpers
{
    public static class DbInitialize
    {
        public static void InitializeDb() 
        {
            Context c = new Context();

            if (!(c.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                c.Database.EnsureCreated();
            }

            if (c.Accounts.Any())
                return;

            c.Accounts.Add(new Account { Name="Ali", Password="demo", Email="demo@demo.com" });
            c.SaveChanges();
        }
    }
}
