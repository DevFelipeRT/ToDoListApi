using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.Commands.Lists;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;
using Domain.Lists;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to create a new To-Do list for a user.
/// Ensures title encapsulation and aggregate consistency.
/// </summary>
public sealed class CreateToDoListHandler : IRequestHandler<CreateToDoListCommand, Guid>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IToDoListTitleUniquenessChecker _titleChecker;

    public CreateToDoListHandler(
        IToDoListRepository listRepository,
        IToDoListTitleUniquenessChecker titleChecker)
    {
        _listRepository = listRepository;
        _titleChecker = titleChecker;
    }

    public async Task<Guid> Handle(CreateToDoListCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = AccountId.FromGuid(request.UserId);
        var title = new Title(request.Title);
        var description = request.Description != null ? new Description(request.Description) : null;

        if (!await _titleChecker.IsTitleUniqueAsync(userId, title, cancellationToken))
            throw new InvalidOperationException("A list with the same title already exists for this user.");

        // Create new list
        var list = new ToDoList(userId, title, description);

        // Persist list
        await _listRepository.AddAsync(list, cancellationToken);

        return list.Id.Value;
    }
}
