using Notes.Application.Notes.Queries.GetNoteList;
using Notes.Tests.Common;
using Shouldly;

namespace Notes.Tests.Queries;

[Collection("QueryCollection")]
public class GetNoteListQueryHandlerTests(QueryTestFixture fixture)
{
    [Fact]
    public async Task GetNoteListQueryHandler_Success()
    {
        // arrange
        var handler = new GetNoteListQueryHandler(fixture.Context, fixture.Mapper);
        
        // act
        var result = await handler.Handle(new GetNoteListQuery
        {
            UserId = NotesContextFactory.UserBId
        }, CancellationToken.None);

        // assert
        result.ShouldBeAssignableTo<NoteListVm>();
        result.Notes.Count.ShouldBe(2);
    }
}