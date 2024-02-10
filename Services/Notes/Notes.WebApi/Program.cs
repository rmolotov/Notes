using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Notes.Application.Common.Mappings;
using Notes.Application.DI;
using Notes.Application.Interfaces;
using Notes.Persistence.Database;
using Notes.Persistence.DI;
using Notes.WebApi.Middleware;
using Notes.WebApi.Services;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Notes.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.File(Path.Combine("Logs", "NotesWebAppLog-.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Host.UseSerilog();

        // Application
        builder.Services
            .AddAutoMapper(config =>
            {
                config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                config.AddProfile(new AssemblyMappingProfile(typeof(INotesDbContext).Assembly));
            })
            .AddApplication()
            .AddPersistence(builder.Configuration)
            .AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            })
            .AddControllers();

        // Auth
        builder.Services
            .AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5001";
                options.Audience = "Notes.Web";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });
        builder.Services
            .AddAuthorizationBuilder()
            .AddPolicy("ApiScopePolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Notes.Web");
                });

        // Versioning and Swagger
        builder.Services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        builder.Services
            .AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>()
            .AddSwaggerGen();

        // Logging
        builder.Services
            .AddSingleton<ICurrentUserService, CurrentUserService>()
            .AddHttpContextAccessor();
        
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            try
            {
                var context = serviceProvider.GetRequiredService<NotesDbContext>();
                DbInitializer.Initialize(context);
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "An error occured on app initialization");
            }
        }

        app
            .UseSwagger()
            .UseSwaggerUI(config =>
            {
                config.RoutePrefix = string.Empty;
                foreach (var description in app.DescribeApiVersions())
                    config.SwaggerEndpoint(
                        $"swagger/{description.GroupName}/swagger.json",
                        $"{description.ApiVersion}");
            })
            .UseCustomExceptionHandler()
            .UseRouting()
            .UseHttpsRedirection()
            .UseCors("AllowAll")
            .UseAuthentication()
            .UseAuthorization()
            
            .UseSerilogRequestLogging();

        app
            .MapControllers()
            .RequireAuthorization("ApiScopePolicy");

        app.Run();
    }
}