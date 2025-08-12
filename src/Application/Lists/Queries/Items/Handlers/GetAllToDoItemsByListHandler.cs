using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.DTOs;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.ValueObjects;
using Domain.Lists.Services.Interfaces;

namespace Application.Lists.Queries.Items.Handlers;

/// <summary>
/// Handles the query to retrieve all To-Do items within a specific To-Do list.
/// </summary>
public sealed class GetAllToDoItemsByListHandler : IRequestHandler<GetAllToDoItemsByListQuery, IReadOnlyCollection<ToDoItemDto>>
{
    private readonly IToDoListRepository _toDoListRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllToDoItemsByListHandler"/> class.
    /// </summary>
    /// <param name="toDoListRepository">Repository for reading To-Do lists and items.</param>
    public GetAllToDoItemsByListHandler(IToDoListRepository toDoListRepository, IAuthorizationService authorizationService)
    {
        _toDoListRepository = toDoListRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<ToDoItemDto>> Handle(GetAllToDoItemsByListQuery request, CancellationToken cancellationToken)
    {
        // Convert primitive data to Value Objects
        var listId = new ToDoListId(request.ListId);
        var userId = AccountId.FromGuid(request.UserId);

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);

        var items = await _toDoListRepository.GetAllItemsByListIdAndUserAsync(listId, userId, cancellationToken);
        
        return items
            .Select(item => ToDoItemDto.FromDomain(item, listId))
            .ToArray();
    }
}
