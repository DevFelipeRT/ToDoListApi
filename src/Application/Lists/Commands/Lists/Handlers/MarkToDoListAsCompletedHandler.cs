using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.Services;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;
using Domain.Accounts.ValueObjects;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to mark a To-Do list as completed.
/// Ensures aggregate existence, user authorization, and state update.
/// </summary>
public sealed class MarkToDoListAsCompletedHandler : IRequestHandler<MarkToDoListAsCompletedCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;

    public MarkToDoListAsCompletedHandler(IToDoListRepository listRepository, IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(MarkToDoListAsCompletedCommand request, CancellationToken cancellationToken)
    {
        // Convert raw IDs to value objects
        var userId = AccountId.FromGuid(request.UserId);
        var listId = new ToDoListId(request.ListId);

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken) ?? throw new InvalidOperationException("List not found.");
        
        list.MarkAsCompleted();

        await _listRepository.UpdateAsync(list, cancellationToken);
        return true;
    }
}
