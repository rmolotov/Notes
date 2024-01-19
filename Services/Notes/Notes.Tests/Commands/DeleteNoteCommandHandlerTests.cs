using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Exceptions;
using Notes.Application.Notes.Commands.CreateNote;
using Notes.Application.Notes.Commands.DeleteNote;
using Notes.Tests.Common;

namespace Notes.Tests.Commands;

public class DeleteNoteCommandHandlerTests : TestCommandBase
{
    [Fact]
    public async Task DeleteNoteCommandHandler_Success()
    {
        // arrange
        var handler = new DeleteNoteCommandHandler(Context);

        // act
        await handler.Handle(new DeleteNoteCommand
            {
                Id = NotesContextFactory.NoteIdForDelete,
                UserId = NotesContextFactory.UserBId
            },
            CancellationToken.None);

        // assert
        Assert.Null(await Context.Notes.SingleOrDefaultAsync(
            note => note.Id == NotesContextFactory.NoteIdForDelete,
            CancellationToken.None));
    }

    [Fact]
    public async Task DeleteNoteCommandHandler_FailOnWrongId()
    {
        // arrange
        var handler = new DeleteNoteCommandHandler(Context);

        // act
        // assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await handler.Handle(new DeleteNoteCommand
                {
                    Id = Guid.NewGuid(),
                    UserId = NotesContextFactory.UserAId
                },
                CancellationToken.None)
        );
    }

    [Fact]
    public async Task DeleteNoteCommandHandler_FailOnWrongUserId()
    {
        // arrange
        var deleteHandler = new DeleteNoteCommandHandler(Context);
        var createHandler = new CreateNoteCommandHandler(Context);
        var noteId = await createHandler.Handle(
            new CreateNoteCommand
            {
                Title = "Note Title",
                Details = "Note Details",
                UserId = NotesContextFactory.UserAId
            },
            CancellationToken.None
        );

        // act
        // assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await deleteHandler.Handle(new DeleteNoteCommand
                {
                    Id = noteId,
                    UserId = NotesContextFactory.UserBId
                },
                CancellationToken.None)
        );
    }
}