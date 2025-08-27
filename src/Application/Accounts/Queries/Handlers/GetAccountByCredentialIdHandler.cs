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
/// Handler responsible for retrieving an account by credential id from the repository and converting to DTO.
/// </summary>
public sealed class GetAccountByCredentialIdHandler : IRequestHandler<GetAccountByCredentialIdQuery, AccountDto?>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByCredentialIdHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    /// <summary>
    /// Handles the query to retrieve an account by credential id.
    /// </summary>
    /// <param name="query">The query containing the account credential id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AccountDto if found; otherwise, null.</returns>
    public async Task<AccountDto?> Handle(GetAccountByCredentialIdQuery query, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var credentialId = CredentialId.FromValue(query.CredentialIdValue);

        var account = await RetrieveAccount(credentialId, cancellationToken);

        if (account == null)
            return null;

        return AccountDto.FromAggregate(account);
    }

    private async Task<Account?> RetrieveAccount(CredentialId credentialId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByCredentialIdAsync(credentialId, cancellationToken);
        return account;
    }
}
