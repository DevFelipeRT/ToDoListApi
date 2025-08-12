using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.DTOs;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;

namespace Application.Lists.Queries.Lists.Handlers;

/// <summary>
/// Handles the query to retrieve all To-Do lists for a specific user.
/// </summary>
public sealed class GetAllToDoListsByUserHandler : IRequestHandler<GetAllToDoListsByUserQuery, IReadOnlyCollection<ToDoListDto>>
{
    private readonly IToDoListRepository _toDoListRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllToDoListsByUserHandler"/> class.
    /// </summary>
    /// <param name="toDoListRepository">Repository for reading To-Do lists.</param>
    public GetAllToDoListsByUserHandler(IToDoListRepository toDoListRepository)
    {
        _toDoListRepository = toDoListRepository;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<ToDoListDto>> Handle(GetAllToDoListsByUserQuery request, CancellationToken cancellationToken)
    {
        var userId = AccountId.FromGuid(request.UserId);
        var toDoLists = await _toDoListRepository.GetAllByUserAsync(userId, cancellationToken);

        return toDoLists
            .Select(ToDoListDto.FromDomain)
            .ToArray();
    }
}
