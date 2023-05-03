using Dashboard.Helpers;
using Dashboard.Models;
using Dashboard.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Adding [Authorize]
builder.Services.AddMvc(c =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    c.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<Context>();
builder.Services.AddTransient<UserService>();


DbInitialize.InitializeDb();

builder.Services
    .AddAuthentication(o => o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(c => c.LoginPath = "/home/login");

builder.Services.AddHangfire(x =>
{
    x
    .UsePostgreSqlStorage(AppConstants.Db.ConnectionString, new PostgreSqlStorageOptions
    {
        PrepareSchemaIfNecessary = true,
        InvisibilityTimeout = TimeSpan.FromMinutes(1)
    })
    .UseRecommendedSerializerSettings();
});


// SchedulePollingInterval : How often Hangfire to check Jobs
builder.Services.AddHangfireServer(ops =>
{

    ops.ServerName = "Standard Web Job Server";
    ops.WorkerCount = 10;
    ops.SchedulePollingInterval = TimeSpan.FromSeconds(20);
    ops.CancellationCheckInterval = TimeSpan.FromSeconds(20);
    ops.Queues = new string[] { "email" };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
AppConstants.AppInfo.IsDevelopment = app.Environment.IsDevelopment();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthenticationFilter() }
});


// Register Hangfire Job, so that, crawling can be repeated based on recurrence...
RecurringJob.AddOrUpdate<HangfireService>(j => j.ExecuteJob(), "0 0 11,14,16,18-21 * * *"); 

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
