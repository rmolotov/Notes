using MediatR;
using Notes.Application.Interfaces;
using Notes.Core.Entities;

namespace Notes.Application.Notes.Commands.CreateNote;

public class CreateNoteCommandHandler(INotesDbContext dbContext) : IRequestHandler<CreateNoteCommand, Guid>
{
    public async Task<Guid> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note
        {
            UserId = request.UserId,
            Title = request.Title,
            Details = request.Details,
            
            Id = Guid.NewGuid(),
            CreatedDate = DateTime.Now,
            EditedDate = null
        };

        await dbContext.Notes.AddAsync(note, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return note.Id;
    }
}