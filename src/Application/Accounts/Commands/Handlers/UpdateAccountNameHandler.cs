using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Repositories;
using Domain.Accounts.Entities;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Handler responsible for updating a account's display name.
/// </summary>
public sealed class UpdateAccountNameHandler : IRequestHandler<UpdateAccountNameCommand>
{
    private readonly IAccountRepository _accountRepository;

    public UpdateAccountNameHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task Handle(UpdateAccountNameCommand command, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var accountId = AccountId.FromGuid(command.AccountId);
        var newName = new AccountName(command.NewName);

        var account = RetrieveAccount(accountId, cancellationToken);

        account.UpdateName(newName);

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
