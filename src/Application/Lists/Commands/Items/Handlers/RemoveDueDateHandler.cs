using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;
using Application.Abstractions.Persistence;

namespace Application.Lists.Commands.Items.Handlers;

/// <summary>
/// Handles removing a due date from a specific to-do item,
/// including validation and domain logic execution.
/// </summary>
public sealed class RemoveDueDateHandler : IRequestHandler<RemoveDueDateCommand>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public RemoveDueDateHandler(IToDoListRepository listRepository, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task Handle(RemoveDueDateCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var listId = new ToDoListId(request.ListId);
        var itemId = new ToDoItemId(request.ItemId);
        var accountId = AccountId.FromGuid(request.AccountId);

        await _authorizationService.AssertAccountListAccessAsync(accountId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken) ?? throw new InvalidOperationException("List not found.");

        var item = list.GetItem(itemId) ?? throw new InvalidOperationException("Item not found.");
        item.RemoveDueDate();

        // Persist changes
        await _listRepository.UpdateAsync(list, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
