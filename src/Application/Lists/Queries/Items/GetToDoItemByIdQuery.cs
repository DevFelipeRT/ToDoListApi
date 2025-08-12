using System;
using MediatR;
using Application.Lists.DTOs; // Ou outro namespace do DTO de resposta

namespace Application.Lists.Queries.Items;

/// <summary>
/// Query to retrieve a To-Do item by its unique identifier and parent list.
/// </summary>
public sealed class GetToDoItemByIdQuery : IRequest<ToDoItemDto?>
{
    /// <summary>
    /// Gets the unique identifier of the parent To-Do list.
    /// </summary>
    public Guid ListId { get; }

    /// <summary>
    /// Gets the unique identifier of the To-Do item.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Gets the unique identifier of the user performing the query.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetToDoItemByIdQuery"/> class.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    public GetToDoItemByIdQuery(Guid listId, Guid itemId, Guid userId)
    {
        ListId = listId;
        ItemId = itemId;
        UserId = userId;
    }
}
