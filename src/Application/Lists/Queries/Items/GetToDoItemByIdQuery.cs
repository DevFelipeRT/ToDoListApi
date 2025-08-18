using System;
using MediatR;
using Application.Lists.DTOs;

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
    /// Gets the unique identifier of the account performing the query.
    /// </summary>
    public Guid AccountId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetToDoItemByIdQuery"/> class.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <param name="accountId">The unique identifier of the account.</param>
    public GetToDoItemByIdQuery(Guid listId, Guid itemId, Guid accountId)
    {
        ListId = listId;
        ItemId = itemId;
        AccountId = accountId;
    }
}
