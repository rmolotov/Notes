using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Notes.Application.Common.Mappings;
using Notes.Application.DI;
using Notes.Application.Interfaces;
using Notes.Persistence.Database;
using Notes.Persistence.DI;
using Notes.WebApi.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Notes.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
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

        builder.Services
            .AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:44392";
                options.Audience = "NotesWebAPI";
                options.RequireHttpsMetadata = false;
            });

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
            
        
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            try
            {
                var context = serviceProvider.GetRequiredService<NotesDbContext>();
                DbInitializer.Initialize(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        app
            .UseCustomExceptionHandler()
            .UseRouting()
            .UseHttpsRedirection()
            .UseCors("AllowAll")
            .UseAuthentication()
            .UseAuthorization()
            .UseSwagger()
            .UseSwaggerUI(config =>
            {
                config.RoutePrefix = string.Empty;
                foreach (var description in app.DescribeApiVersions())
                    config.SwaggerEndpoint(
                        $"swagger/{description.GroupName}/swagger.json",
                        $"{description.ApiVersion}");
            });
        
        app.MapControllers();

        app.Run();
    }
}