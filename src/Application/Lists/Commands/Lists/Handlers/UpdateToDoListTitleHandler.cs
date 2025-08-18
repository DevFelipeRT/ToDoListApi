using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;
using Domain.Lists.Policies;
using Application.Abstractions.Persistence;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to update the title of a To-Do list.
/// Ensures aggregate existence, account authorization, and value object encapsulation.
/// </summary>
public sealed class UpdateToDoListTitleHandler : IRequestHandler<UpdateToDoListTitleCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;
    private readonly IToDoListUniquenessPolicy _uniquenessChecker;

    public UpdateToDoListTitleHandler(IToDoListRepository listRepository, IUnitOfWork unitOfWork, IAuthorizationService authorizationService, IToDoListUniquenessPolicy uniquenessChecker)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
        _uniquenessChecker = uniquenessChecker;
    }

    public async Task<bool> Handle(UpdateToDoListTitleCommand request, CancellationToken cancellationToken)
    {
        // Convert raw IDs to value objects
        var accountId = AccountId.FromGuid(request.AccountId);
        var listId = new ToDoListId(request.ListId);
        var newTitle = new Title(request.NewTitle);

        await _authorizationService.AssertAccountListAccessAsync(accountId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken) ?? throw new InvalidOperationException("List not found.");

        if (!await _uniquenessChecker.IsTitleUniqueAsync(list.AccountId, newTitle, cancellationToken))
            throw new InvalidOperationException("List title already exists for this account.");

        list.UpdateTitle(newTitle);
        
        await _listRepository.UpdateAsync(list, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
