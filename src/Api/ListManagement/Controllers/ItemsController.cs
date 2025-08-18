using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Lists.Commands.Items;
using Application.Lists.Queries.Items;
using Application.Lists.DTOs;
using Api.Common.Controllers;
using Api.Common.Contracts;
using Api.ListManagement.Contracts.Requests;
using Api.ListManagement.Contracts.Responses;

namespace Api.ListManagement.Controllers;

/// <summary>
/// Controller for managing to-do items within lists.
/// </summary>
[ApiController]
[Route("api/lists/{listId:guid}/items")]
[Authorize]
public sealed class ItemsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling commands and queries.</param>
    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new to-do item in the specified list.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="request">The request containing item details.</param>
    /// <returns>The created to-do item.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ToDoItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<ToDoItemResponse>>> Create(
        Guid listId,
        [FromBody] CreateToDoItemRequest request)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new CreateToDoItemCommand(listId, accountId, request.Title, request.DueDate);
            var createdItemId = await _mediator.Send(command);

            // Create response with the returned ID
            var response = new ToDoItemResponse
            {
                Id = createdItemId,
                Title = request.Title,
                IsComplete = false,
                CreatedAt = DateTime.UtcNow,
                DueDate = request.DueDate,
                CompletedAt = null,
                ListId = listId
            };

            return Created(response, $"/api/lists/{listId}/items/{createdItemId}");
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Gets all to-do items from the specified list.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="pageNumber">The page number for pagination.</param>
    /// <param name="pageSize">The page size for pagination.</param>
    /// <returns>A paginated list of to-do items.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ToDoItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(
        Guid listId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var query = new GetAllToDoItemsByListQuery(listId, accountId);
            var items = await _mediator.Send(query);

            if (items == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "List not found or no access"
                });
            }

            var responses = items.Select(MapToItemResponse).ToList();
            
            var totalCount = responses.Count;
            var paginatedItems = responses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new ApiResponse<List<ToDoItemResponse>>
            {
                Success = true,
                Data = paginatedItems,
                Pagination = new PageInfo
                {
                    TotalItems = totalCount,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Gets a specific to-do item by its ID.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>The to-do item if found.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ToDoItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid listId, Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var query = new GetToDoItemByIdQuery(listId, id, accountId);
            var item = await _mediator.Send(query);

            if (item == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Item not found"
                });
            }

            var response = MapToItemResponse(item);
            return Ok(new ApiResponse<ToDoItemResponse>
            {
                Success = true,
                Data = response
            });
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Updates the title of a to-do item.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="request">The request containing the new title.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}/title")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTitle(
        Guid listId,
        Guid id,
        [FromBody] UpdateToDoItemTitleRequest request)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new UpdateToDoItemTitleCommand(listId, id, accountId, request.Title);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Marks a to-do item as completed.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsCompleted(Guid listId, Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new MarkAsCompletedCommand(listId, id, accountId);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Marks a to-do item as incomplete.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}/incomplete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsIncomplete(Guid listId, Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new MarkAsIncompleteCommand(listId, id, accountId);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Deletes a to-do item.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid listId, Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new DeleteToDoItemCommand(listId, id, accountId);
            await _mediator.Send(command);

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Transfers a to-do item to another list.
    /// </summary>
    /// <param name="listId">The unique identifier of the source list.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="request">The request containing the target list ID.</param>
    /// <returns>Success response if transfer is completed.</returns>
    [HttpPut("{id:guid}/transfer")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Transfer(
        Guid listId, 
        Guid id, 
        [FromBody] TransferToDoItemRequest request)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new TransferItemCommand 
            { 
                AccountId = accountId,
                SourceListId = listId, 
                ItemId = id, 
                TargetListId = request.TargetListId 
            };
            await _mediator.Send(command);
            
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Item transferred successfully"
            });
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Sets a due date for a todo item.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="request">The request containing the due date date.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id:guid}/due-date")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetDueDate(
        Guid listId,
        Guid id,
        [FromBody] SetDueDateRequest request)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new SetDueDateCommand 
            { 
                AccountId = accountId,
                ListId = listId, 
                ItemId = id, 
                DueDate = request.DueDate 
            };
            await _mediator.Send(command);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Removes a due date from a todo item.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="id">The unique identifier of the item.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id:guid}/due-date")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveDueDate(Guid listId, Guid id)
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var command = new RemoveDueDateCommand 
            { 
                AccountId = accountId,
                ListId = listId, 
                ItemId = id 
            };
            await _mediator.Send(command);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Gets all items with active due dates for the current account.
    /// </summary>
    /// <returns>A list of items with due dates.</returns>
    [HttpGet("~/api/items/due-dates")]
    [ProducesResponseType(typeof(ApiResponse<List<ToDoItemResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItemsWithDueDates()
    {
        try
        {
            var accountId = GetCurrentAccountId();
            var query = new GetItemsWithDueDatesQuery { AccountId = accountId };
            var items = await _mediator.Send(query);

            var responses = items.Select(MapToItemResponse).ToList();
            return Ok(new ApiResponse<List<ToDoItemResponse>>
            {
                Success = true,
                Data = responses
            });
        }
        catch (Exception ex)
        {
            return HandleApiException(ex);
        }
    }

    /// <summary>
    /// Maps a ToDoItemDto to a ToDoItemResponse.
    /// </summary>
    /// <param name="dto">The ToDoItemDto to map.</param>
    /// <returns>The mapped ToDoItemResponse.</returns>
    private static ToDoItemResponse MapToItemResponse(ToDoItemDto dto)
    {
        return new ToDoItemResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            IsComplete = dto.IsCompleted,
            CreatedAt = dto.CreatedAt,
            DueDate = dto.DueDate,
            CompletedAt = dto.CompletedAt,
            ListId = dto.ListId
        };
    }
}
