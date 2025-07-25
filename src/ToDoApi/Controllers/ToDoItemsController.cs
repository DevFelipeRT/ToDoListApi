using MediatR;
using Microsoft.AspNetCore.Mvc;
using ToDoList.ToDoApi.Contracts.ToDoItems;
using ToDoList.Application.Dtos;
using ToDoList.Application.ToDoItems.Queries.GetAllToDoItems;
using ToDoList.Application.ToDoItems.Queries.GetToDoItem;
using ToDoList.Application.ToDoItems.Commands.CreateToDoItem;
using ToDoList.Application.ToDoItems.Commands.UpdateToDoItem;
using ToDoList.Application.ToDoItems.Commands.DeleteToDoItem;

namespace ToDoList.ToDoApi.Controllers;

/// <summary>
/// API controller for managing To-Do items.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ToDoItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoItemsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    public ToDoItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new To-Do item.
    /// </summary>
    /// <param name="request">The request object containing the To-Do item's data.</param>
    /// <returns>An HTTP 201 Created response if successful.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateToDoItemRequest request)
    {
        var command = new CreateToDoItemCommand(request.Title);

        await _mediator.Send(command);

        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Retrieves all To-Do items.
    /// </summary>
    /// <returns>A list of all To-Do items.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ToDoItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllToDoItemsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all To-Do items with HATEOAS links.
    /// </summary>
    /// <returns>A collection of To-Do items with HATEOAS links.</returns>
    [HttpGet("hateoas")]
    [ProducesResponseType(typeof(ToDoItemHateoasCollectionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllHateoas()
    {
        var query = new GetAllToDoItemsQuery();
        var items = await _mediator.Send(query);

        var response = new ToDoItemHateoasCollectionResponse
        {
            Items = items.Select(item => new ToDoItemHateoasResponse
            {
                Id = item.Id,
                Title = item.Title,
                IsCompleted = item.IsCompleted,
                CreatedAt = item.CreatedAt,
                CompletedAt = item.CompletedAt,
                Links = new List<LinkDto>
                {
                    new() { Rel = "self", Href = Url.Action(nameof(GetByIdHateoas), new { id = item.Id })!, Method = "GET" },
                    new() { Rel = "update-title", Href = Url.Action(nameof(UpdateTitle), new { id = item.Id })!, Method = "PUT" },
                    new() { Rel = "delete", Href = Url.Action(nameof(Delete), new { id = item.Id })!, Method = "DELETE" },
                    new() { Rel = item.IsCompleted ? "mark-incomplete" : "mark-completed", Href = Url.Action(item.IsCompleted ? nameof(MarkAsIncomplete) : nameof(MarkAsCompleted), new { id = item.Id })!, Method = "PATCH" }
                }
            }).ToList(),
            Links = new List<LinkDto>
            {
                new() { Rel = "self", Href = Url.Action(nameof(GetAllHateoas))!, Method = "GET" },
                new() { Rel = "create", Href = Url.Action(nameof(Create))!, Method = "POST" }
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a To-Do item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    /// <returns>The To-Do item if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ToDoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetToDoItemQuery(id);
        var result = await _mediator.Send(query);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a To-Do item by its unique identifier with HATEOAS links.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    /// <returns>The To-Do item with HATEOAS links if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{id}/hateoas")]
    [ProducesResponseType(typeof(ToDoItemHateoasResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdHateoas(int id)
    {
        var query = new GetToDoItemQuery(id);
        var result = await _mediator.Send(query);
        if (result is null)
            return NotFound();

        var response = new ToDoItemHateoasResponse
        {
            Id = result.Id,
            Title = result.Title,
            IsCompleted = result.IsCompleted,
            CreatedAt = result.CreatedAt,
            CompletedAt = result.CompletedAt,
            Links = new List<LinkDto>
            {
                new() { Rel = "self", Href = Url.Action(nameof(GetByIdHateoas), new { id })!, Method = "GET" },
                new() { Rel = "update-title", Href = Url.Action(nameof(UpdateTitle), new { id })!, Method = "PUT" },
                new() { Rel = "delete", Href = Url.Action(nameof(Delete), new { id })!, Method = "DELETE" },
                new() { Rel = result.IsCompleted ? "mark-incomplete" : "mark-completed", Href = Url.Action(result.IsCompleted ? nameof(MarkAsIncomplete) : nameof(MarkAsCompleted), new { id })!, Method = "PATCH" }
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Marks a To-Do item as completed.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    /// <returns>204 No Content if successful; 404 Not Found if the item does not exist.</returns>
    [HttpPatch("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsCompleted(int id)
    {
        var command = new MarkAsCompletedCommand(id);
        var success = await _mediator.Send(command);
        if (!success)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Marks a To-Do item as incomplete.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    /// <returns>204 No Content if successful; 404 Not Found if the item does not exist.</returns>
    [HttpPatch("{id}/incomplete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsIncomplete(int id)
    {
        var command = new MarkAsIncompleteCommand(id);
        var success = await _mediator.Send(command);
        if (!success)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Updates the title of a To-Do item.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    /// <param name="request">The request containing the new title.</param>
    /// <returns>204 No Content if successful; 404 Not Found if the item does not exist.</returns>
    [HttpPut("{id}/title")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTitle(int id, [FromBody] UpdateToDoItemTitleRequest request)
    {
        var command = new UpdateToDoItemTitleCommand(id, request.Title);
        var success = await _mediator.Send(command);
        if (!success)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes a To-Do item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    /// <returns>204 No Content if successful; 404 Not Found if the item does not exist.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteToDoItemCommand(id);
        var success = await _mediator.Send(command);
        if (!success)
            return NotFound();
        return NoContent();
    }
}