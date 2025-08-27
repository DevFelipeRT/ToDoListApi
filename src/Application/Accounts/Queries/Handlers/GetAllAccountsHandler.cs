using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.Entities;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries.Handlers;

/// <summary>
/// Handler responsible for retrieving all accounts from the repository and converting them to DTOs.
/// </summary>
public sealed class GetAllAccountsHandler : IRequestHandler<GetAllAccountsQuery, IReadOnlyCollection<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAllAccountsHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    /// <summary>
    /// Handles the query to retrieve all accounts.
    /// </summary>
    /// <param name="query">The query object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of AccountDto representing all accounts.</returns>
    public async Task<IReadOnlyCollection<AccountDto>> Handle(GetAllAccountsQuery query, CancellationToken cancellationToken)
    {
        var accounts = await RetrieveAllAccounts(cancellationToken);

        return ConvertToDto(accounts);
    }

    private async Task<IReadOnlyCollection<Account>> RetrieveAllAccounts(CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);
        return accounts ?? new List<Account>();
    }

    private IReadOnlyCollection<AccountDto> ConvertToDto(IReadOnlyCollection<Account> accounts)
    {
        var dtos = new List<AccountDto>();
        foreach (var account in accounts)
            dtos.Add(AccountDto.FromAggregate(account));
        return dtos;
    }
}
