using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Lists.DTOs;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;
using MediatR;

namespace Application.Lists.Queries.Items.Handlers;

/// <summary>
/// Handles the query to retrieve all to-do items with active due dates for a specific user.
/// </summary>
public sealed class GetItemsWithDueDatesHandler : IRequestHandler<GetItemsWithDueDatesQuery, IEnumerable<ToDoItemDto>>
{
    private readonly IToDoListRepository _toDoListRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetItemsWithDueDatesHandler"/> class.
    /// </summary>
    /// <param name="toDoListRepository">Repository for reading To-Do lists and items.</param>
    public GetItemsWithDueDatesHandler(IToDoListRepository toDoListRepository)
    {
        _toDoListRepository = toDoListRepository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ToDoItemDto>> Handle(GetItemsWithDueDatesQuery request, CancellationToken cancellationToken)
    {
        // Convert primitive data to Value Objects
        var userId = AccountId.FromGuid(request.UserId);

        var tuples = await _toDoListRepository.GetItemsWithDueDateAndListIdAsync( userId, request.FromDate, request.ToDate, cancellationToken);

        return tuples.Select(t => ToDoItemDto.FromDomain(t.Item, t.ListId));
    }
}
