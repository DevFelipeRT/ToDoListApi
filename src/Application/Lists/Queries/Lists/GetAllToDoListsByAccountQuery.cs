using System;
using System.Collections.Generic;
using MediatR;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Lists;

/// <summary>
/// Query to retrieve all To-Do lists for a specific account.
/// </summary>
public sealed class GetAllToDoListsByAccountQuery : IRequest<IReadOnlyCollection<ToDoListDto>>
{
    /// <summary>
    /// Gets the unique identifier of the account.
    /// </summary>
    public Guid AccountId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllToDoListsByAccountQuery"/> class.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    public GetAllToDoListsByAccountQuery(Guid accountId)
    {
        AccountId = accountId;
    }
}
