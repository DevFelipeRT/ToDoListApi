using System;
using System.Collections.Generic;
using MediatR;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Items;

/// <summary>
/// Query to get all to-do items with active due dates for a specific account.
/// </summary>
public sealed class GetItemsWithDueDatesQuery : IRequest<IEnumerable<ToDoItemDto>>
{
    /// <summary>
    /// Gets the unique identifier of the account.
    /// </summary>
    public Guid AccountId { get; init; }

    /// <summary>
    /// Gets the optional start date to filter reminders (inclusive).
    /// </summary>
    public DateTime? FromDate { get; init; }

    /// <summary>
    /// Gets the optional end date to filter reminders (inclusive).
    /// </summary>
    public DateTime? ToDate { get; init; }
}
