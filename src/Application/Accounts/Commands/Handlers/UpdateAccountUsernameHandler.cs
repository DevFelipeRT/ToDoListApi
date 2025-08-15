using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Policies.Interfaces;
using Domain.Accounts.Entities;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Handler responsible for updating a account's username.
/// </summary>
public sealed class UpdateAccountUsernameHandler : IRequestHandler<UpdateAccountUsernameCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountUniquenessPolicy _uniquenessChecker;

    public UpdateAccountUsernameHandler(
        IAccountRepository accountRepository,
        IAccountUniquenessPolicy uniquenessChecker)
    {
        _accountRepository = accountRepository;
        _uniquenessChecker = uniquenessChecker;
    }

    public async Task Handle(UpdateAccountUsernameCommand command, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var accountId = AccountId.FromGuid(command.AccountId);
        var newUsername = new AccountUsername(command.NewUsername);

        var account = RetrieveAccount(accountId, cancellationToken);

        await ValidateNewUsernameUniqueness(newUsername, cancellationToken);

        account.UpdateUsername(newUsername);

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
    
    private async Task ValidateNewUsernameUniqueness(AccountUsername username, CancellationToken cancellationToken)
    {
        if (!await _uniquenessChecker.IsUsernameUniqueAsync(username, cancellationToken))
            throw new InvalidOperationException("Username already in use.");
    }
}
