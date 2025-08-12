using System;
using System.Collections.Generic;
using Application.Lists.DTOs;
using MediatR;

namespace Application.Lists.Queries.Items;

/// <summary>
/// Query to get all to-do items with active due dates for a specific user.
/// </summary>
public sealed class GetItemsWithDueDatesQuery : IRequest<IEnumerable<ToDoItemDto>>
{
    /// <summary>
    /// Gets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the optional start date to filter reminders (inclusive).
    /// </summary>
    public DateTime? FromDate { get; init; }

    /// <summary>
    /// Gets the optional end date to filter reminders (inclusive).
    /// </summary>
    public DateTime? ToDate { get; init; }
}
