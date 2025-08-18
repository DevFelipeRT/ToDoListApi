using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Repositories;
using Domain.Accounts.Entities;
using Application.Abstractions.Persistence;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Handler responsible for deactivating an account.
/// </summary>
public sealed class DeactivateAccountHandler : IRequestHandler<DeactivateAccountCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeactivateAccountCommand command, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var accountId = AccountId.FromGuid(command.AccountId);

        var account = RetrieveAccount(accountId, cancellationToken);

        account.Deactivate();

        // Persist changes
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    private Account RetrieveAccount(AccountId accountId, CancellationToken cancellationToken)
    {
        var account = _accountRepository.GetByIdAsync(accountId, cancellationToken).Result;
        if (account == null)
            throw new InvalidOperationException("Account not found.");
        return account;
    }
}
