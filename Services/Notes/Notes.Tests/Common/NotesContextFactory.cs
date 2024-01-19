using Microsoft.EntityFrameworkCore;
using Notes.Core.Entities;
using Notes.Persistence.Database;

namespace Notes.Tests.Common;

public class NotesContextFactory
{
    public static Guid UserAId = Guid.NewGuid();
    public static Guid UserBId = Guid.NewGuid();
    
    public static Guid NoteIdForUpdate = Guid.NewGuid();
    public static Guid NoteIdForDelete = Guid.NewGuid();

    public static NotesDbContext Create()
    {
        var options = new DbContextOptionsBuilder<NotesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new NotesDbContext(options);
        context.Database.EnsureCreated();

        context.Notes.AddRange(
            new Note
            {
                Id = Guid.Parse("A4ED0A36-AD3B-4D6B-8B58-DECA07E49AB4"),
                UserId = UserAId,
                CreatedDate = DateTime.Today,
                EditedDate = null,
                Title = "Title1",
                Details = "Details1"
            },
            new Note
            {
                Id = Guid.Parse("AB563262-3F52-442D-82EB-B9BEED8C6FA3"),
                UserId = UserBId,
                CreatedDate = DateTime.Today,
                EditedDate = null,
                Title = "Title2",
                Details = "Details2"
            },
            new Note
            {
                Id = NoteIdForUpdate,
                UserId = UserAId,
                CreatedDate = DateTime.Today,
                EditedDate = null,
                Title = "Title3",
                Details = "Details3"
            },
            new Note
            {
                Id = NoteIdForDelete,
                UserId = UserBId,
                CreatedDate = DateTime.Today,
                EditedDate = null,
                Title = "Title4",
                Details = "Details4"
            }
        );
        context.SaveChanges();
        
        return context;
    }

    public static void Destroy(NotesDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}