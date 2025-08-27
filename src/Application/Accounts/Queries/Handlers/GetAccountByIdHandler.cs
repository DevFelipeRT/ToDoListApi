using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Entities;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries.Handlers;

/// <summary>
/// Handler responsible for retrieving an account by id from the repository and converting to DTO.
/// </summary>
public sealed class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    /// <summary>
    /// Handles the query to retrieve an account by id.
    /// </summary>
    /// <param name="query">The query containing the account id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AccountDto if found; otherwise, null.</returns>
    public async Task<AccountDto?> Handle(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var accountId = AccountId.FromGuid(query.AccountId);

        var account = await RetrieveAccount(accountId, cancellationToken);

        if (account == null)
            return null;

        return AccountDto.FromAggregate(account);
    }

    private async Task<Account?> RetrieveAccount(AccountId accountId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        return account;
    }
}
