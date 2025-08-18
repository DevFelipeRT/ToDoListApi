using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.ValueObjects;
using Domain.Lists.Services.Interfaces;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Items.Handlers;

/// <summary>
/// Handles the query to retrieve a To-Do item by its unique identifier and parent list.
/// </summary>
public sealed class GetToDoItemByIdHandler : IRequestHandler<GetToDoItemByIdQuery, ToDoItemDto?>
{
    private readonly IToDoListRepository _toDoListRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetToDoItemByIdHandler"/> class.
    /// </summary>
    /// <param name="toDoListRepository">Repository for reading To-Do lists and items.</param>
    public GetToDoItemByIdHandler(IToDoListRepository toDoListRepository, IAuthorizationService authorizationService)
    {
        _toDoListRepository = toDoListRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc/>
    public async Task<ToDoItemDto?> Handle(GetToDoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var listId = new ToDoListId(request.ListId);
        var itemId = new ToDoItemId(request.ItemId);
        var accountId = AccountId.FromGuid(request.AccountId);

        await _authorizationService.AssertAccountListAccessAsync(accountId, listId, cancellationToken);

        var item = await _toDoListRepository.GetItemByIdAsync(listId, itemId, accountId, cancellationToken);
        if (item is null)
            return null;

        return ToDoItemDto.FromDomain(item, listId);
    }
}
