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
/// Ensures aggregate existence, user authorization, and value object encapsulation.
/// </summary>
public sealed class UpdateToDoListTitleHandler : IRequestHandler<UpdateToDoListTitleCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IToDoListTitleUniquenessChecker _titleChecker;

    public UpdateToDoListTitleHandler(IToDoListRepository listRepository, IAuthorizationService authorizationService, IToDoListTitleUniquenessChecker titleChecker)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
        _titleChecker = titleChecker;
    }

    public async Task<bool> Handle(UpdateToDoListTitleCommand request, CancellationToken cancellationToken)
    {
        // Convert raw IDs to value objects
        var userId = AccountId.FromGuid(request.UserId);
        var listId = new ToDoListId(request.ListId);
        var newTitle = new Title(request.NewTitle);

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken) ?? throw new InvalidOperationException("List not found.");

        if (!await _titleChecker.IsTitleUniqueAsync(list.UserId, newTitle, cancellationToken))
            throw new InvalidOperationException("List title already exists for this user.");

        list.UpdateTitle(newTitle);
        
        await _listRepository.UpdateAsync(list, cancellationToken);
        return true;
    }
}
