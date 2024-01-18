namespace Notes.Identity.Database;

public static class DbInitializer
{
    public static void Initialize(AuthDbContext context) => 
        context.Database.EnsureCreated();
}