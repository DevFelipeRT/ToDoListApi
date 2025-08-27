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
/// Handler responsible for retrieving an account by email from the repository and converting to DTO.
/// </summary>
public sealed class GetAccountByEmailHandler : IRequestHandler<GetAccountByEmailQuery, AccountDto?>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByEmailHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    /// <summary>
    /// Handles the query to retrieve an account by email.
    /// </summary>
    /// <param name="query">The query containing the account email.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AccountDto if found; otherwise, null.</returns>
    public async Task<AccountDto?> Handle(GetAccountByEmailQuery query, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var email = new AccountEmail(query.Email);

        var account = await RetrieveAccountByEmail(email, cancellationToken);

        if (account == null)
            return null;

        return AccountDto.FromAggregate(account);
    }

    private async Task<Account?> RetrieveAccountByEmail(AccountEmail email, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByEmailAsync(email, cancellationToken);
        if (account == null)
            return null;
        return account;
    }
}
