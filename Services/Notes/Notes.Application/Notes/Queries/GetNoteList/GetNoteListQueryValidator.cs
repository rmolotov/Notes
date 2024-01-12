using FluentValidation;

namespace Notes.Application.Notes.Queries.GetNoteList;

public class GetNoteListQueryValidator : AbstractValidator<GetNoteListQuery>
{
    public GetNoteListQueryValidator()
    {
        RuleFor(query => query.UserId)
            .NotEqual(Guid.Empty);
    }
}