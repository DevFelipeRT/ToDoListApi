using System;
using System.Collections.Generic;
using MediatR;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Items;

/// <summary>
/// Query to retrieve all To-Do items within a specific To-Do list.
/// </summary>
public sealed class GetAllToDoItemsByListQuery : IRequest<IReadOnlyCollection<ToDoItemDto>>
{
    /// <summary>
    /// Gets the unique identifier of the parent To-Do list.
    /// </summary>
    public Guid ListId { get; }

    /// <summary>
    /// Gets the unique identifier of the account performing the query.
    /// </summary>
    public Guid AccountId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllToDoItemsByListQuery"/> class.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="accountId">The unique identifier of the account.</param>
    public GetAllToDoItemsByListQuery(Guid listId, Guid accountId)
    {
        ListId = listId;
        AccountId = accountId;
    }
}
