using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Notes.Commands.UpdateNote;
using Notes.Tests.Common;

namespace Notes.Tests.Commands;

public class UpdateNoteCommandHandlerTests : TestCommandBase
{
    [Fact]
    public async Task UpdateNoteCommandHandler_Success()
    {
        // arrange
        var handler = new UpdateNoteCommandHandler(Context);
        var updatedTitle = "New Title";

        // act
        await handler.Handle(new UpdateNoteCommand()
            {
                Id = NotesContextFactory.NoteIdForUpdate,
                UserId = NotesContextFactory.UserAId,
                Title = updatedTitle
            },
            CancellationToken.None);

        // assert
        Assert.NotNull(Context.Notes.SingleOrDefaultAsync(
            note => note.Id == NotesContextFactory.NoteIdForUpdate && note.Title == updatedTitle,
            CancellationToken.None));
    }
    
    [Fact]
    public async Task UpdateNoteCommandHandler_FailOnWrongId()
    {
        // arrange
        var handler = new UpdateNoteCommandHandler(Context);
        var updatedTitle = "New Title";

        // act
        // assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await handler.Handle(new UpdateNoteCommand
                {
                    Id = Guid.NewGuid(),
                    UserId = NotesContextFactory.UserAId,
                    Title = updatedTitle
                },
                CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateNoteCommandHandler_FailOnWrongUserId()
    {
        // arrange
        var updateHandler = new UpdateNoteCommandHandler(Context);
        var updatedTitle = "New Title";

        // act
        // assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await updateHandler.Handle(new UpdateNoteCommand
                {
                    Id = NotesContextFactory.NoteIdForUpdate,
                    UserId = NotesContextFactory.UserBId,
                    Title = updatedTitle
                },
                CancellationToken.None)
        );
    }
}