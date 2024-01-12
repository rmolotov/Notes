using System.Reflection;
using Notes.Application.Common.Mappings;
using Notes.Application.DI;
using Notes.Application.Interfaces;
using Notes.Persistence.Database;
using Notes.Persistence.DI;
using Notes.WebApi.Middleware;

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
            .UseCors();
        
        app.MapControllers();

        app.Run();
    }
}