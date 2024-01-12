using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notes.Application.Interfaces;
using Notes.Persistence.Database;

namespace Notes.Persistence.DI;

public static class DependencyInjection
{
    private const string SqliteConnectionString = "Sqlite";
    
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(SqliteConnectionString);

        services
            .AddDbContext<NotesDbContext>(options => 
                options.UseSqlite(connectionString));
        services
            .AddScoped<INotesDbContext>(provider =>
                provider.GetService<NotesDbContext>()
            );
        
        return services;
    }
}