using Notes.Application.Notes.Queries.GetNoteDetails;
using Notes.Tests.Common;
using Shouldly;

namespace Notes.Tests.Queries;

[Collection("QueryCollection")]
public class GetNoteDetailsQueryHandlerTests(QueryTestFixture fixture)
{
    [Fact]
    public async Task GetNoteDetailsQueryHandler_Success()
    {
        // arrange
        var handler = new GetNoteDetailsQueryHandler(fixture.Context, fixture.Mapper);
        
        // act
        var result = await handler.Handle(new GetNoteDetailsQuery
        {
            Id = Guid.Parse("AB563262-3F52-442D-82EB-B9BEED8C6FA3"),
            UserId = NotesContextFactory.UserBId
        }, CancellationToken.None);

        // assert
        result.ShouldBeAssignableTo<NoteDetailsVm>();
        result.Title.ShouldBe("Title2");
        result.CreatedDate.ShouldBe(DateTime.Today);
    }
}