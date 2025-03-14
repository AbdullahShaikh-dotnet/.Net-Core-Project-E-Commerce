using E_Commerce.DataAccess.Data;
using ECom.DataAccess.Data.DbInitializer;
using ECom.DataAccess.Repository;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility.Interface;
using ECom.Utility.Services;
using ECom.Utility.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuestPDF.Infrastructure;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


// Added Serilog Logger
//Log.Logger = new LoggerConfiguration()
//    .Enrich.FromLogContext()                          // Adds contextual information to logs
//    .Enrich.WithProperty("Application", "E-Commerce")      // Adds custom property to logs
//    .WriteTo.Console()                                // Logs to console
//    .WriteTo.File("logs/warning-log.txt",             // Logs warnings and above to this file
//                  rollingInterval: RollingInterval.Day,
//                  restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
//    .WriteTo.File("logs/error-log.txt",               // Logs errors and above to this file
//                  rollingInterval: RollingInterval.Day,
//                  restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
//    .CreateLogger();

//builder.Host.UseSerilog();
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


// Add services to the container.
builder.Services.AddControllersWithViews();


string pcName = Environment.MachineName;
string DockerContainerID = "bd403b8e8402"; 
string ConnectionStringName = pcName == "MERA-PC" ? $"{pcName}Connection" : "DefaultConnection";

if (pcName == DockerContainerID) // Docker ContainerID
    ConnectionStringName = "DockerConnection";

string? ConnectionString = builder.Configuration.GetConnectionString(ConnectionStringName);


builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(ConnectionString));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = $"/Identity/Account/Logout";
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


builder.Services.AddAuthentication().AddFacebook(option =>
{
    var FacebookSettings = builder.Configuration.GetSection("Facebook").Get<FacebookSettings>();
    option.AppId = FacebookSettings?.AppID;
    option.AppSecret = FacebookSettings?.AppSecret;

    // Handle if User Cancel the External Login request
    option.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
    {
        OnRemoteFailure = context =>
        {
            context.HandleResponse(); // Prevents the exception
            context.Response.Redirect("/Identity/Account/Login?error=FacebookAuthCancelled");
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthentication().AddGoogle(option =>
{
    var GoogleSettings = builder.Configuration.GetSection("Google").Get<GoogleSettings>();
    option.ClientId = GoogleSettings?.ClientID;
    option.ClientSecret = GoogleSettings?.ClientSecret;

    // Handle if User Cancel the External Login request
    option.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
    {
        OnRemoteFailure = context =>
        {
            context.HandleResponse(); // Prevents the exception
            context.Response.Redirect("/Identity/Account/Login?error=GoogleAuthCancelled");
            return Task.CompletedTask;
        },
    };
});



builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(100);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
}); // Configured Session


builder.Services.AddRazorPages();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.Configure<RazorPaySettings>(builder.Configuration.GetSection("Razorpay"));
builder.Services.AddSingleton<IRazorPayService, RazorPayService>();

builder.Services.Configure<MailJetSettings>(builder.Configuration.GetSection("MailJet"));
builder.Services.AddSingleton<IMailJetService, MailJetService>();


QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddScoped<IInvoiceService, InvoiceService>();


builder.Services.AddSingleton<IConnectionMultiplexer>(data =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetValue<string>(pcName == DockerContainerID ? "RedisDockerConnection" : "RedisConnection")));

builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IWebSocketManager, ECom.Utility.Services.WebSocketManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



// Websocket Integration
app.UseWebSockets();
using (var scope = app.Services.CreateScope())
{
    var webSocketManager = scope.ServiceProvider.GetRequiredService<IWebSocketManager>();

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/ws")
        {
            await webSocketManager.HandleConnection(context);
        }
        else
        {
            await next();
        }
    });
};



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Added Session
InIt_Database(); // Initialize Database
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");


app.Run();



void InIt_Database()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}