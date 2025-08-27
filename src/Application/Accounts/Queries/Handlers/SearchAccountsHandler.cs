using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Entities;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries.Handlers;

/// <summary>
/// Handler responsible for searching accounts with optional filters and pagination.
/// </summary>
public sealed class SearchAccountsHandler : IRequestHandler<SearchAccountsQuery, IReadOnlyCollection<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;

    public SearchAccountsHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    /// <summary>
    /// Handles the search query and returns a paginated list of AccountDto.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of AccountDto matching the criteria.</returns>
    public async Task<IReadOnlyCollection<AccountDto>> Handle(SearchAccountsQuery query, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var name = query.Name != null ? new AccountName(query.Name) : null;
        var username = query.Username != null ? new AccountUsername(query.Username) : null;
        var email = query.Email != null ? new AccountEmail(query.Email) : null;
        var isActive = query.IsActive.HasValue ? (bool)query.IsActive : (bool?)null;

        var accounts = await SearchAsync(
            name,
            username,
            email,
            isActive,
            query.Page,
            query.PageSize,
            cancellationToken);

        var dtos = ConvertToDtos(accounts);

        return dtos;
    }

    private async Task<IReadOnlyCollection<Account>> SearchAsync(
        AccountName? name,
        AccountUsername? username,
        AccountEmail? email,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await _accountRepository.SearchAsync(name, username, email, isActive, page, pageSize, cancellationToken);
    }

    private IReadOnlyCollection<AccountDto> ConvertToDtos(IReadOnlyCollection<Account> accounts)
    {
        var dtos = new List<AccountDto>();

        foreach (var account in accounts)
        {
            dtos.Add(AccountDto.FromAggregate(account));
        }

        return dtos;
    }
}
