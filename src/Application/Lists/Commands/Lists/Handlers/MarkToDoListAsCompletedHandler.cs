using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.Services;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;
using Domain.Accounts.ValueObjects;
using Application.Abstractions.Persistence;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to mark a To-Do list as completed.
/// Ensures aggregate existence, account authorization, and state update.
/// </summary>
public sealed class MarkToDoListAsCompletedHandler : IRequestHandler<MarkToDoListAsCompletedCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public MarkToDoListAsCompletedHandler(IToDoListRepository listRepository, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(MarkToDoListAsCompletedCommand request, CancellationToken cancellationToken)
    {
        // Convert raw IDs to value objects
        var accountId = AccountId.FromGuid(request.AccountId);
        var listId = new ToDoListId(request.ListId);

        await _authorizationService.AssertAccountListAccessAsync(accountId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken) ?? throw new InvalidOperationException("List not found.");
        
        list.MarkAsCompleted();

        await _listRepository.UpdateAsync(list, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
