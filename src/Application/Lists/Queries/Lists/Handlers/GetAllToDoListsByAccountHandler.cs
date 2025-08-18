using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Lists.Handlers;

/// <summary>
/// Handles the query to retrieve all To-Do lists for a specific account.
/// </summary>
public sealed class GetAllToDoListsByAccountHandler : IRequestHandler<GetAllToDoListsByAccountQuery, IReadOnlyCollection<ToDoListDto>>
{
    private readonly IToDoListRepository _toDoListRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllToDoListsByAccountHandler"/> class.
    /// </summary>
    /// <param name="toDoListRepository">Repository for reading To-Do lists.</param>
    public GetAllToDoListsByAccountHandler(IToDoListRepository toDoListRepository)
    {
        _toDoListRepository = toDoListRepository;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<ToDoListDto>> Handle(GetAllToDoListsByAccountQuery request, CancellationToken cancellationToken)
    {
        var accountId = AccountId.FromGuid(request.AccountId);
        var toDoLists = await _toDoListRepository.GetAllByAccountAsync(accountId, cancellationToken);

        return toDoLists
            .Select(ToDoListDto.FromDomain)
            .ToArray();
    }
}
