using System;
using MediatR;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Lists;

/// <summary>
/// Query to retrieve a To-Do list by its unique identifier and account.
/// </summary>
public sealed class GetToDoListByIdQuery : IRequest<ToDoListDto?>
{
    /// <summary>
    /// Gets the unique identifier of the To-Do list.
    /// </summary>
    public Guid ListId { get; }

    /// <summary>
    /// Gets the unique identifier of the account.
    /// </summary>
    public Guid AccountId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetToDoListByIdQuery"/> class.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="accountId">The unique identifier of the account.</param>
    public GetToDoListByIdQuery(Guid listId, Guid accountId)
    {
        ListId = listId;
        AccountId = accountId;
    }
}
