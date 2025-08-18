using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.ValueObjects;
using Domain.Lists.Services.Interfaces;
using Application.Lists.DTOs;

namespace Application.Lists.Queries.Lists.Handlers;

/// <summary>
/// Handles the query to retrieve a To-Do list by its unique identifier and account.
/// </summary>
public sealed class GetToDoListByIdHandler : IRequestHandler<GetToDoListByIdQuery, ToDoListDto?>
{
    private readonly IToDoListRepository _toDoListRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetToDoListByIdHandler"/> class.
    /// </summary>
    /// <param name="toDoListRepository">Repository for reading To-Do lists.</param>
    public GetToDoListByIdHandler(IToDoListRepository toDoListRepository, IAuthorizationService authorizationService)
    {
        _toDoListRepository = toDoListRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc/>
    public async Task<ToDoListDto?> Handle(GetToDoListByIdQuery request, CancellationToken cancellationToken)
    {
        var listId = new ToDoListId(request.ListId);
        var accountId = AccountId.FromGuid(request.AccountId);

        await _authorizationService.AssertAccountListAccessAsync(accountId, listId, cancellationToken);

        var toDoList = await _toDoListRepository.GetByIdAsync(listId, cancellationToken);

        if (toDoList is null)
            return null;

        var dto = ToDoListDto.FromDomain(toDoList);

        return dto;
    }
}
