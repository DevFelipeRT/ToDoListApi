using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;
using Domain.Accounts.ValueObjects;
using Application.Abstractions.Persistence;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to delete a To-Do list.
/// Ensures aggregate existence, account authorization, and removal.
/// </summary>
public sealed class DeleteToDoListHandler : IRequestHandler<DeleteToDoListCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public DeleteToDoListHandler(IToDoListRepository listRepository, IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(DeleteToDoListCommand request, CancellationToken cancellationToken)
    {
        // Convert raw IDs to value objects
        var accountId = AccountId.FromGuid(request.AccountId);
        var listId = new ToDoListId(request.ListId);

        await _authorizationService.AssertAccountListAccessAsync(accountId, listId, cancellationToken);
        
        await _listRepository.DeleteAsync(listId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
