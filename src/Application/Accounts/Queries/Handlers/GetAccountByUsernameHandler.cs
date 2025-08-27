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
/// Handler responsible for retrieving a account by username from the repository and converting to DTO.
/// </summary>
public sealed class GetAccountByUsernameHandler : IRequestHandler<GetAccountByUsernameQuery, AccountDto?>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByUsernameHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    /// <summary>
    /// Handles the query to retrieve a account by username.
    /// </summary>
    /// <param name="query">The query containing the username.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AccountDto if found; otherwise, null.</returns>
    public async Task<AccountDto?> Handle(GetAccountByUsernameQuery query, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var username = new AccountUsername(query.Username);

        var account = await RetrieveAccount(username, cancellationToken);

        if (account == null)
            return null;

        return AccountDto.FromAggregate(account);
    }

    private async Task<Account?> RetrieveAccount(AccountUsername username, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByUsernameAsync(username, cancellationToken);
        return account;
    }
}
