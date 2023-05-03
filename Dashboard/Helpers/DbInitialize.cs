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

            // Some Demo/Dummy data to create here...
            c.Accounts.Add(new Account { Name = "Ali", Email = "demo@demo.com", Password = "demo" });
            c.SaveChanges();

            c.Stories.Add(new Story
            {
                Title = "Demo Event-1",
                Address = "Asgard",
                Description = "this is a demo event",
                Keywords = new List<string> { "keyword1", "keyword2" }.ToArray(),
                InstagramUrl = "https://www.instagram.com/some_instagram_account/",
                CoverPhotoURL = "https://alicakil.com/files/me.jpg",
                CreatedById = 1,
                CreatedAt= DateTime.Today,
            });

            c.Stories.Add(new Story
            {
                Title = "Demo Event-2",
                Address = "Neptun",
                Description = "this is a demo event",
                Keywords = new List<string> { "keyword1", "keyword2" }.ToArray(),
                InstagramUrl = "https://www.instagram.com/some_instagram_account/",
                CoverPhotoURL = "https://alicakil.com/files/me.jpg",
                CreatedById = 1,
                CreatedAt = DateTime.Today.AddDays(1),
            });

            c.Stories.Add(new Story
            {
                Title = "Demo Event-3",
                Address = "Andromeda",
                Description = "this is a demo event",
                Keywords = new List<string> { "keyword1", "keyword2" }.ToArray(),
                InstagramUrl = "https://www.instagram.com/some_instagram_account/",
                CoverPhotoURL = "https://alicakil.com/files/me.jpg",
                CreatedById = 1,
                CreatedAt = DateTime.Today.AddDays(2),
            });

            c.Stories.Add(new Story
            {
                Title = "Demo Event-4",
                Address = "Milkway",
                Description = "this is a demo event",
                Keywords = new List<string> { "keyword1", "keyword2" }.ToArray(),
                InstagramUrl = "https://www.instagram.com/some_instagram_account/",
                CoverPhotoURL = "https://alicakil.com/files/me.jpg",
                CreatedById = 1,
                CreatedAt = DateTime.Today.AddDays(3),

            });
            c.SaveChanges();
        }
    }
}
