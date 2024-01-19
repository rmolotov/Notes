using Microsoft.EntityFrameworkCore;
using Notes.Application.Notes.Commands.CreateNote;
using Notes.Tests.Common;

namespace Notes.Tests.Commands;

public class CreateNoteCommandHandlerTests : TestCommandBase
{
    [Fact]
    public async Task CreateNoteCommandHandler_Success()
    {
        // arrange
        var handler = new CreateNoteCommandHandler(Context);
        var noteName = "Note Name";
        var noteDetails = "Note Details";
        
        // act
        var noteId = await handler.Handle(
            new CreateNoteCommand
            {
                Title = noteName,
                Details = noteDetails,
                UserId = NotesContextFactory.UserAId
            },
            CancellationToken.None);

        // assert
        Assert.NotNull(
            await Context.Notes.SingleOrDefaultAsync(note => 
                note.Id == noteId && note.Title == noteName && note.Details == noteDetails,
                CancellationToken.None));
    }
}