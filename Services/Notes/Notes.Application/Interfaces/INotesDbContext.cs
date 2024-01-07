using Microsoft.EntityFrameworkCore;
using Notes.Core.Entities;

namespace Notes.Application.Interfaces;

public interface INotesDbContext
{
    DbSet<Note> Notes { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}