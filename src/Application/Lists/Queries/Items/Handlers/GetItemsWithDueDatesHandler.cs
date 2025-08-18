using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Items.Handlers;

/// <summary>
/// Handles the query that retrieves to-do items with active due dates for a given account.
/// Orchestrates: (1) fetch account's list IDs; (2) fetch items by IN(ListId) and optional date window.
/// </summary>
public sealed class GetItemsWithDueDatesHandler
    : IRequestHandler<GetItemsWithDueDatesQuery, IEnumerable<ToDoItemDto>>
{
    private readonly IToDoListRepository _toDoListRepository;

    public GetItemsWithDueDatesHandler(IToDoListRepository toDoListRepository)
    {
        _toDoListRepository = toDoListRepository ?? throw new ArgumentNullException(nameof(toDoListRepository));
    }

    public async Task<IEnumerable<ToDoItemDto>> Handle(
        GetItemsWithDueDatesQuery request,
        CancellationToken cancellationToken)
    {
        var accountId = AccountId.FromGuid(request.AccountId);

        var listIds = await _toDoListRepository.GetIdsByAccountAsync(accountId, cancellationToken);
        if (listIds is null || listIds.Count == 0)
            return Enumerable.Empty<ToDoItemDto>();

        var tuples = await _toDoListRepository.GetItemsWithDueDateByListIdsAsync(
            listIds,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return tuples.Select(t => ToDoItemDto.FromDomain(t.Item, t.ListId)).ToList();
    }
}
