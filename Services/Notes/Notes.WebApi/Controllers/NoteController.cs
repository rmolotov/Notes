using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.Notes.Commands.CreateNote;
using Notes.Application.Notes.Commands.DeleteNote;
using Notes.Application.Notes.Commands.UpdateNote;
using Notes.Application.Notes.Queries.GetNoteDetails;
using Notes.Application.Notes.Queries.GetNoteList;
using Notes.WebApi.Models;

namespace Notes.WebApi.Controllers;

[ApiVersionNeutral]
[Authorize(AuthenticationSchemes = "Bearer")]
[Produces("application/json")]
[Route("api/{version:apiVersion}/[controller]")]
public class NoteController(IMapper mapper) : BaseController
{
    /// <summary>
    /// Get all notes linked with current user
    /// </summary>
    /// <remarks>
    /// Request sample:
    /// GET /note
    /// </remarks>
    /// <returns>NoteListVm</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet]
    public async Task<ActionResult<NoteListVm>> GetAll()
    {
        var query = new GetNoteListQuery
        {
            UserId = UserId
        };
        var vm = await Mediator.Send(query);
        return Ok(vm);
    }

    /// <summary>
    /// Get note by id, linked with current user
    /// </summary>
    /// <remarks>
    /// Request sample:
    /// GET /note/B4AF7564-24A3-4FF1-AEE4-8AAD5DCB6441
    /// </remarks>
    /// <param name="id">Note Id (GUID)</param>
    /// <returns>NoteDetailsVm</returns>
    /// <response code="200">Success</response>
    /// <response code="401">If user is unauthorized</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NoteDetailsVm>> Get(Guid id)
    {
        var query = new GetNoteDetailsQuery
        {
            Id = id,
            UserId = UserId
        };
        var vm = await Mediator.Send(query);
        return Ok(vm);
    }

    /// <summary>
    /// Create new note
    /// </summary>
    /// <remarks>
    /// Request sample:
    /// POST /note
    /// {
    ///     title = "Note Title",
    ///     details = "Note Details"
    /// }
    /// </remarks>
    /// <returns>Created note Id (GUID)</returns>
    /// <response code="201">Created</response>
    /// <response code="401">If user is unauthorized</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateNoteDto dto)
    {
        var command = mapper.Map<CreateNoteCommand>(dto);
        command.UserId = UserId;

        var id = await Mediator.Send(command);

        return Created($"[/api/notes/{id}]", id);
    }

    /// <summary>
    /// Edit existed note by Id
    /// </summary>
    /// <remarks>
    /// Request sample:
    /// PUT /note/2587430E-58F1-4B54-998F-43C5B4825617
    /// {
    ///     title = "Note new title",
    ///     details = "Note new details"
    /// }
    /// </remarks>
    /// <returns>No content</returns>
    /// <response code="204">Updated</response>
    /// <response code="401">If user is unauthorized</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut]
    public async Task<ActionResult> Update([FromBody] UpdateNoteDto dto)
    {
        var command = mapper.Map<UpdateNoteCommand>(dto);
        command.UserId = UserId;

        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Delete existed note by Id
    /// </summary>
    /// <remarks>
    /// Request sample:
    /// DELETE /note/2587430E-58F1-4B54-998F-43C5B4825617
    /// </remarks>
    /// <returns>No content</returns>
    /// <response code="204">Deleted</response>
    /// <response code="401">If user is unauthorized</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete]
    public async Task<ActionResult> Delete([FromBody] Guid id)
    {
        var command = new DeleteNoteCommand
        {
            Id = id,
            UserId = UserId
        };

        await Mediator.Send(command);

        return NoContent();
    }
}