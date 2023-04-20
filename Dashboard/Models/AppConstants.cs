using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;
using System;

namespace Dashboard.Models
{
    public static class AppConstants
    {
        public static class Instagram
        {
            public const string username = "your instagram account";
            public const string password = "your instagram password";
            public const string RootUrl = "https://www.instagram.com/";
        }

        public static class Db
        {
            public const string ConnectionString = "Server=localhost;Port=5432;Database=selenium_crawler_demo;User Id=postgres;Password=13;";
        }

        public static class AWS
        {
            public const string accessKey = "your-access-key";
            public const string secretKey = "your-secret-key";
            public const string Region = "eu-north-1";
            public const string PublicBucket = "your-unique-bucket-name";
            public const string ImgRoot = "https://s3." + Region + ".amazonaws.com/" + PublicBucket + "/";
        }

        public static class AppInfo
        {
            public static Version VersionInfo { get; } = new Version(1,0);
            public static string Name { get; } = "Selenium Crawler App";
            public static string Description { get; } = "This is a demo app to show how to use selenium driver to crawl web sites";
            public static string Author { get; } = "Ali CAKIL";
            public static bool IsDevelopment { get; set; } = true;
            public static int MaxRecsPerPage { get; set; } = 12;


            public static void CreateDatabase()
            {
                Context c = new Context();

                // Check db if exists
                if ((c.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                {
                    c.Database.EnsureDeleted();
                }

                if (!(c.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                {
                    c.Database.EnsureCreated();
                }

                if (c.Accounts.Any()) // means we already initialized db before for the demo
                    return;

                try
                {
                    // Some Demo data to create...
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


        }

    }


}
