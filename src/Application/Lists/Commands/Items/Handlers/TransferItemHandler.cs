using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.Commands.Items;
using Domain.Lists.Services.Interfaces;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Accounts.Repositories;

namespace Application.Lists.Commands.Items.Handlers;

/// <summary>
/// Handles transferring a to-do item from one list to another,
/// including validation, authorization, domain logic execution, and persistence.
/// </summary>
public sealed class TransferItemHandler : IRequestHandler<TransferItemCommand>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IToDoListItemTransferService _itemTransferService;

    public TransferItemHandler(
        IToDoListRepository listRepository,
        IUserRepository userRepository,
        IAuthorizationService authorizationService,
        IToDoListItemTransferService itemTransferService)
    {
        _listRepository = listRepository;
        _userRepository = userRepository;
        _authorizationService = authorizationService;
        _itemTransferService = itemTransferService;
    }

    public async Task Handle(TransferItemCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = new AccountId(request.UserId);
        var sourceListId = new ToDoListId(request.SourceListId);
        var targetListId = new ToDoListId(request.TargetListId);
        var itemId = new ToDoItemId(request.ItemId);

        await _authorizationService.AssertUserListAccessAsync(userId, sourceListId, cancellationToken);
        await _authorizationService.AssertUserListAccessAsync(userId, targetListId, cancellationToken);

        var sourceList = await _listRepository.GetByIdAsync(sourceListId, cancellationToken)
            ?? throw new InvalidOperationException("Source list not found.");
        var targetList = await _listRepository.GetByIdAsync(targetListId, cancellationToken)
            ?? throw new InvalidOperationException("Target list not found.");

        var item = sourceList.GetItem(itemId)
            ?? throw new InvalidOperationException("Item not found in source list.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        var transferred = _itemTransferService.TransferToDoItem(user, item, sourceList, targetList);
        if (!transferred)
            throw new InvalidOperationException("Could not transfer the item.");

        // Persist changes
        await _listRepository.UpdateAsync(sourceList, cancellationToken);
        await _listRepository.UpdateAsync(targetList, cancellationToken);
    }
}
