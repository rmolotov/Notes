using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Interfaces;
using Notes.Core.Entities;

namespace Notes.Application.Notes.Commands.UpdateNote;

public class UpdateNoteCommandHandler(INotesDbContext dbContext) : IRequestHandler<UpdateNoteCommand>
{
    public async Task Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Notes.FirstOrDefaultAsync(
            e => e.Id == request.Id, 
            cancellationToken);

        if (entity == null || entity.UserId != request.UserId)
            throw new NotFoundException(nameof(Note), request.Id);

        entity.Title = request.Title;
        entity.Details = request.Details;
        entity.EditedDate = DateTime.Now;
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}