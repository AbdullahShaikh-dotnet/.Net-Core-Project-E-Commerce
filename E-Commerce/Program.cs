using AspNetCore.ReCaptcha;
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
using Azure.Identity;
using static Org.BouncyCastle.Math.EC.ECCurve;

var builder = WebApplication.CreateBuilder(args);


///<summary>
/// Below chain builds config from multiple sources in this order:
/// appsettings.json, User Secrets, Env Variables
/// UserSecrets Path : C:\Users\[USerName]\AppData\Roaming\Microsoft\UserSecrets\51d0634e-c8b6-4026-94c1-5443f893aadd\Secret.json
/// This GUID(51d0634e-c8b6-4026-94c1-5443f893aadd) Added in .csproj file <UserSecretsId>51d0634e-c8b6-4026-94c1-5443f893aadd</UserSecretsId>
///</summary>
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>() // This line is critical
    .AddEnvironmentVariables();


// Server Loggging
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


// Mis-Configuration throws Error on Buid Time Insted of Run Time
builder.Host.UseDefaultServiceProvider((context, option) =>
{
    option.ValidateScopes = true;
    option.ValidateOnBuild = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

var isRunningInDockerContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";


// Now: Read connection string based on environment
string connectionString;

if (isRunningInDockerContainer)
{
    // Build dynamic Docker connection string
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

    connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id=sa;Password={dbPassword};TrustServerCertificate=True;";
}
else
{
    // Local Development
    string pcName = Environment.MachineName;
    string connectionStringName = pcName == "MERA-PC" ? $"{pcName}Connection" : "DefaultConnection";

    connectionString = builder.Configuration.GetConnectionString(connectionStringName);
}


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);


// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = $"/Identity/Account/Logout";
    option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


// Facebook External Login
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

// Google External Login
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

// Google Recaptcha
builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));


// Services
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


// Register Redis Connection if Failed Skip the Redis Connection
builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
{
    try
    {
        var configKey = isRunningInDockerContainer ? "RedisDockerConnection" : "RedisConnection";
        var redisConnStr = builder.Configuration[configKey];
        return ConnectionMultiplexer.Connect(redisConnStr);
    }
    catch
    {
        // Redis connection failed, skip registration
        return null;
    }
});


builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IWebSocketManager, ECom.Utility.Services.WebSocketManager>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddRazorPages();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(100);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
}); // Configured Session



var app = builder.Build();
ApplyMigrationsAndSeed();


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
//InIt_Database(); // Initialize Database
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void ApplyMigrationsAndSeed()
{
    using var scope = app.Services.CreateScope();
    // For Docker Only
    //var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //dbContext.Database.Migrate();

    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}
