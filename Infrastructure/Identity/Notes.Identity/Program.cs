using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Notes.Identity;
using Notes.Identity.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<AuthDbContext>(
        options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")))
    .AddIdentity<AppUser, IdentityRole>(
        config =>
        {
            config.Password.RequiredLength = 4;
            config.Password.RequireDigit = false;
            config.Password.RequireNonAlphanumeric = false;
            config.Password.RequireUppercase = false;
        })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .ConfigureApplicationCookie(config =>
    {
        config.Cookie.Name = "Notes.Identity.cookie";
        config.LoginPath = "/auth/login";
        config.LogoutPath = "/auth/logout";
    });

builder.Services
    .AddIdentityServer()
    .AddAspNetIdentity<AppUser>()
    .AddInMemoryApiResources(Configuration.ApiResources)
    .AddInMemoryIdentityResources(Configuration.IdentityResources)
    .AddInMemoryApiScopes(Configuration.ApiScopes)
    .AddInMemoryClients(Configuration.Clients)
    .AddDeveloperSigningCredential();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<AuthDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger>();
        logger.LogError(ex, "An error occured on app initialization");
        throw;
    }
}


app
    .UseRouting()
    .UseIdentityServer();

app.MapGet("/", () => "Hello World!");

app.Run();