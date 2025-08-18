using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.Services;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to update the title of a To-Do list.
/// Ensures aggregate existence, account authorization, and value object encapsulation.
/// </summary>
public sealed class UpdateToDoListDescriptionHandler : IRequestHandler<UpdateToDoListDescriptionCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;

    public UpdateToDoListDescriptionHandler(IToDoListRepository listRepository, IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(UpdateToDoListDescriptionCommand request, CancellationToken cancellationToken)
    {
        // Convert raw IDs to value objects
        var accountId = AccountId.FromGuid(request.AccountId);
        var listId = new ToDoListId(request.ListId);
        var newDescription = request.NewDescription != null ? new Description(request.NewDescription) : null;

        await _authorizationService.AssertAccountListAccessAsync(accountId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken) ?? throw new InvalidOperationException("List not found.");

        list.UpdateDescription(newDescription);
        
        await _listRepository.UpdateAsync(list, cancellationToken);
        return true;
    }
}
