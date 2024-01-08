using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Interfaces;

namespace Notes.Application.Notes.Queries.GetNoteList;

public class GetNoteListQueryHandler(INotesDbContext dbContext, IMapper mapper) 
    : IRequestHandler<GetNoteListQuery, NoteListVm>
{
    public async Task<NoteListVm> Handle(GetNoteListQuery request, CancellationToken cancellationToken)
    {
        var query = await dbContext.Notes
            .Where(n => n.UserId == request.UserId)
            .ProjectTo<NoteLookupDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new NoteListVm { Notes = query };
    }
}