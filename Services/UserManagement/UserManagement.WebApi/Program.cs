using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using UserManagement.Core.Entities;
using UserManagement.Persistence.Database;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<UsersDbContext>(optionsBuilder =>
    {
        optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("UsersConnection"));
    });

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<UsersDbContext>()
    .AddDefaultTokenProviders();
builder.Services
    .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("IdentityUri");
        options.RequireHttpsMetadata = false;
    });
builder.Services
    .AddAuthorizationBuilder()
    .AddPolicy("ApiScopePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "Notes.Api");
    });

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

IdentityModelEventSource.ShowPII = true;

app
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization();

app
    .MapControllers()
    .RequireAuthorization("ApiScopePolicy");

app.Run();