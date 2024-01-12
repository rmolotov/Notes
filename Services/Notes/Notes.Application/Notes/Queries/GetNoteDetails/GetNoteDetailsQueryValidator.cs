using FluentValidation;

namespace Notes.Application.Notes.Queries.GetNoteDetails;

public class GetNoteDetailsQueryValidator : AbstractValidator<GetNoteDetailsQuery>
{
    public GetNoteDetailsQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEqual(Guid.Empty);
        RuleFor(query => query.UserId)
            .NotEqual(Guid.Empty);
    }
}