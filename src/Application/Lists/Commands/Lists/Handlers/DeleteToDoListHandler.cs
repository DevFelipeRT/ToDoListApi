using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;
using Domain.Accounts.ValueObjects;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to delete a To-Do list.
/// Ensures aggregate existence, user authorization, and removal.
/// </summary>
public sealed class DeleteToDoListHandler : IRequestHandler<DeleteToDoListCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;

    public DeleteToDoListHandler(IToDoListRepository listRepository, IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(DeleteToDoListCommand request, CancellationToken cancellationToken)
    {
        // Convert raw IDs to value objects
        var userId = new AccountId(request.UserId);
        var listId = new ToDoListId(request.ListId);

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);
        
        await _listRepository.DeleteAsync(listId, cancellationToken);
        return true;
    }
}
