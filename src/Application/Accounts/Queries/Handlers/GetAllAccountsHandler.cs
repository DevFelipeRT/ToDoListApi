using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Repositories;
using Domain.Accounts.Entities;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries.Handlers;

/// <summary>
/// Handler responsible for retrieving all accounts from the repository and converting them to DTOs.
/// </summary>
public sealed class GetAllAccountsHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetAllAccountsHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    /// <summary>
    /// Handles the query to retrieve all accounts.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of AccountDto representing all accounts.</returns>
    public async Task<IReadOnlyCollection<AccountDto>> Handle(CancellationToken cancellationToken)
    {
        var accounts = await RetrieveAllAccounts(cancellationToken);

        return ConvertToDto(accounts);
    }

    private async Task<IReadOnlyCollection<Account>> RetrieveAllAccounts(CancellationToken cancellationToken)
    {
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);
        if (accounts == null || accounts.Count == 0)
            throw new InvalidOperationException("No accounts found.");
        return accounts;
    }

    private IReadOnlyCollection<AccountDto> ConvertToDto(IReadOnlyCollection<Account> accounts)
    {
        var dtos = new List<AccountDto>();
        foreach (var account in accounts)
            dtos.Add(AccountDto.FromAggregate(account));
        return dtos;
    }
}
