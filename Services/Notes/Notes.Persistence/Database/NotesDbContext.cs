using Microsoft.EntityFrameworkCore;
using Notes.Application.Interfaces;
using Notes.Core.Entities;
using Notes.Persistence.EntityTypeConfigurations;

namespace Notes.Persistence.Database;

public class NotesDbContext : DbContext, INotesDbContext
{
    public DbSet<Note> Notes { get; set; }

    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NoteConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}