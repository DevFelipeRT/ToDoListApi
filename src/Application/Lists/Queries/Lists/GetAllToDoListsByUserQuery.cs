using System;
using System.Collections.Generic;
using MediatR;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Lists;

/// <summary>
/// Query to retrieve all To-Do lists for a specific user.
/// </summary>
public sealed class GetAllToDoListsByUserQuery : IRequest<IReadOnlyCollection<ToDoListDto>>
{
    /// <summary>
    /// Gets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllToDoListsByUserQuery"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    public GetAllToDoListsByUserQuery(Guid userId)
    {
        UserId = userId;
    }
}
