using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.Services.Interfaces;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Policies.Interfaces;
using Domain.Accounts.Entities;

namespace Application.Accounts.Commands.Handlers;

/// <summary>
/// Handler responsible for creating a new account.
/// Contains the application logic for account registration including validation and persistence.
/// </summary>
public sealed class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountUniquenessPolicy _uniquenessChecker;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicy _passwordPolicyValidator;

    public CreateAccountHandler(
        IAccountRepository accountRepository,
        IAccountUniquenessPolicy uniquenessChecker,
        IPasswordHasher passwordHasher,
        IPasswordPolicy passwordPolicyValidator)
    {
        _accountRepository = accountRepository;
        _uniquenessChecker = uniquenessChecker;
        _passwordHasher = passwordHasher;
        _passwordPolicyValidator = passwordPolicyValidator;
    }

    public async Task<Guid> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var email = new AccountEmail(command.Email);
        var username = new AccountUsername(command.Username);
        var name = new AccountName(command.Name);

        await ValidateUniqueness(email, username, cancellationToken);

        ValidatePasswordPolicy(command.PlainPassword);

        var passwordHash = GeneratePasswordHash(command.PlainPassword);

        var account = CreateAccountInstance(email, username, name, passwordHash);

        await _accountRepository.AddAsync(account, cancellationToken);

        return account.Id.Value;
    }

    private async Task ValidateUniqueness(AccountEmail email, AccountUsername username, CancellationToken cancellationToken)
    {
        if (!await _uniquenessChecker.IsEmailUniqueAsync(email, cancellationToken))
            throw new InvalidOperationException("Email already in use.");

        if (!await _uniquenessChecker.IsUsernameUniqueAsync(username, cancellationToken))
            throw new InvalidOperationException("Username already in use.");
    }

    private void ValidatePasswordPolicy(string plainPassword)
    {
        if (!_passwordPolicyValidator.IsValid(plainPassword, out var reason))
            throw new InvalidOperationException($"Invalid password: {reason}");
    }

    private string GeneratePasswordHash(string plainPassword)
    {
        return _passwordHasher.HashPassword(plainPassword);
    }
    
    private Account CreateAccountInstance(
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        string passwordHash)
    {
        return Account.Create(email, username, name, passwordHash);
    }
}