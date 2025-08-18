using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Common.Controllers;
using Api.Common.Contracts;
using Api.ListManagement.Contracts.Requests;
using Api.ListManagement.Contracts.Responses;
using Application.Lists.Commands.Lists;
using Application.Lists.Queries.Lists;
using Application.Lists.DTOs;

namespace Api.ListManagement.Controllers;

/// <summary>
/// API controller for managing todo lists. All endpoints require authentication.
/// </summary>
[ApiController]
[Route("api/lists")]
[Authorize]
public class ListsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListsController"/> class.
    /// </summary>
    public ListsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new todo list for the authenticated account.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ListResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ListResponse>>> Create([FromBody] CreateListRequest request)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new CreateToDoListCommand(accountId, request.Title, request.Description);
            var listId = await _mediator.Send(command);

            var createdList = await _mediator.Send(new GetToDoListByIdQuery(listId, accountId));
            if (createdList == null)
                return Error<ListResponse>(new List<string> { "Failed to retrieve created list" });

            var response = MapToListResponse(createdList);
            return Created(response, $"/api/lists/{listId}");
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Retrieves all todo lists for the authenticated account, with optional pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedListResponse>>> GetAll([FromQuery] PaginationParameters parameters)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var query = new GetAllToDoListsByAccountQuery(accountId);
            var result = await _mediator.Send(query);

            var paginatedItems = result.Skip((parameters.Page - 1) * parameters.PageSize)
                                      .Take(parameters.PageSize)
                                      .ToList();

            var response = new PagedListResponse
            {
                Items = paginatedItems.Select(MapToListResponse),
                Pagination = new PageInfo
                {
                    CurrentPage = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalPages = (int)Math.Ceiling((double)result.Count / parameters.PageSize),
                    TotalItems = result.Count
                }
            };

            AddPaginationLinks(response, parameters);
            return Success(response);
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Retrieves a specific todo list by its identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ListResponse>>> GetById(Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var query = new GetToDoListByIdQuery(id, accountId);
            var list = await _mediator.Send(query);

            if (list == null)
            {
                return NotFound(new ApiResponse<ListResponse>
                {
                    Success = false,
                    Message = "List not found"
                });
            }

            var response = MapToListResponse(list);
            return Success(response);
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Updates the title of a todo list.
    /// </summary>
    [HttpPut("{id:guid}/title")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateTitle(Guid id, [FromBody] UpdateListTitleRequest request)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new UpdateToDoListTitleCommand(id, accountId, request.Title);
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "List not found"
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Updates the description of a todo list.
    /// </summary>
    [HttpPut("{id:guid}/description")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateDescription(Guid id, [FromBody] UpdateListDescriptionRequest request)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new UpdateToDoListDescriptionCommand(id, accountId, request.Description);
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "List not found"
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Marks a todo list as completed.
    /// </summary>
    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsCompleted(Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new MarkToDoListAsCompletedCommand(id, accountId);
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "List not found"
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Marks a todo list as incomplete.
    /// </summary>
    [HttpPut("{id:guid}/incomplete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsIncomplete(Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new MarkToDoListAsIncompleteCommand(id, accountId);
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "List not found"
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Deletes a todo list.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new DeleteToDoListCommand(id, accountId);
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "List not found"
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    // ----- HATEOAS & Mapping -----

    private static ListResponse MapToListResponse(ToDoListDto dto)
    {

        var response = new ListResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            IsComplete = dto.IsCompleted,
            ItemCount = dto.Items.Count,
            CompletedItemCount = dto.Items.Count(i => i.IsCompleted),
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.CreatedAt
        };

        AddHateoasLinks(response);
        return response;
    }

    private static void AddHateoasLinks(ListResponse response)
    {
        response.Links = new List<Link>
        {
            new() { Href = $"/api/lists/{response.Id}", Rel = "self", Method = "GET" },
            new() { Href = $"/api/lists/{response.Id}/title", Rel = "update-title", Method = "PUT" },
            new() { Href = $"/api/lists/{response.Id}/complete", Rel = "mark-complete", Method = "PUT" },
            new() { Href = $"/api/lists/{response.Id}/incomplete", Rel = "mark-incomplete", Method = "PUT" },
            new() { Href = $"/api/lists/{response.Id}", Rel = "delete", Method = "DELETE" },
            new() { Href = $"/api/lists/{response.Id}/items", Rel = "items", Method = "GET" }
        };
    }

    private static void AddPaginationLinks(PagedListResponse response, PaginationParameters parameters)
    {
        response.Links = new List<Link>
        {
            new() { Href = $"/api/lists?page={parameters.Page}&pageSize={parameters.PageSize}", Rel = "self", Method = "GET" }
        };

        if (parameters.Page > 1)
        {
            response.Links.Add(new Link
            {
                Href = $"/api/lists?page={parameters.Page - 1}&pageSize={parameters.PageSize}",
                Rel = "previous",
                Method = "GET"
            });
        }

        if (parameters.Page < response.Pagination?.TotalPages)
        {
            response.Links.Add(new Link
            {
                Href = $"/api/lists?page={parameters.Page + 1}&pageSize={parameters.PageSize}",
                Rel = "next",
                Method = "GET"
            });
        }
    }
}
