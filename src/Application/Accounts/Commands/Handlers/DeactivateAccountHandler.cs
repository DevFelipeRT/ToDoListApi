using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Repositories;
using Domain.Accounts.Entities;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Handler responsible for deactivating an account.
/// </summary>
public sealed class DeactivateAccountHandler : IRequestHandler<DeactivateAccountCommand>
{
    private readonly IAccountRepository _accountRepository;

    public DeactivateAccountHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task Handle(DeactivateAccountCommand command, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var accountId = AccountId.FromGuid(command.AccountId);

        var account = RetrieveAccount(accountId, cancellationToken);

        account.Deactivate();

        // Persist changes
        await _accountRepository.UpdateAsync(account, cancellationToken);
    }
    
    private Account RetrieveAccount(AccountId accountId, CancellationToken cancellationToken)
    {
        var account = _accountRepository.GetByIdAsync(accountId, cancellationToken).Result;
        if (account == null)
            throw new InvalidOperationException("Account not found.");
        return account;
    }
}
