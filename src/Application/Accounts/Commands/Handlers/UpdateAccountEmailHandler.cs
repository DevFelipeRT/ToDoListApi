using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Policies.Interfaces;
using Domain.Accounts.Entities;
using Application.Abstractions.Persistence;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Handler responsible for updating a account's email address.
/// </summary>
public sealed class UpdateAccountEmailHandler : IRequestHandler<UpdateAccountEmailCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccountUniquenessPolicy _uniquenessChecker;

    public UpdateAccountEmailHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        IAccountUniquenessPolicy uniquenessChecker)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _uniquenessChecker = uniquenessChecker;
    }

    public async Task Handle(UpdateAccountEmailCommand command, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var accountId = AccountId.FromGuid(command.AccountId);
        var newEmail = new AccountEmail(command.NewEmail);

        var account = RetrieveAccount(accountId, cancellationToken);

        await ValidateNewEmailUniqueness(newEmail, cancellationToken);

        account.UpdateEmail(newEmail);

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

    private async Task ValidateNewEmailUniqueness(AccountEmail email, CancellationToken cancellationToken)
    {
        if (!await _uniquenessChecker.IsEmailUniqueAsync(email, cancellationToken))
            throw new InvalidOperationException("Email already in use.");
    }
}
