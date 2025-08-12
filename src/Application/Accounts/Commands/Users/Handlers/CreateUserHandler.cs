using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts;
using Domain.Accounts.Repositories;
using Domain.Accounts.Services.Interfaces;
using Domain.Accounts.ValueObjects;

namespace Application.Accounts.Commands.Users.Handlers;

/// <summary>
/// Handler responsible for creating a new user account.
/// Contains the application logic for user registration including validation and persistence.
/// </summary>
public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountUniquenessChecker _uniquenessChecker;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicyValidator _passwordPolicyValidator;

    public CreateUserHandler(
        IUserRepository userRepository,
        IAccountUniquenessChecker uniquenessChecker,
        IPasswordHasher passwordHasher,
        IPasswordPolicyValidator passwordPolicyValidator)
    {
        _userRepository = userRepository;
        _uniquenessChecker = uniquenessChecker;
        _passwordHasher = passwordHasher;
        _passwordPolicyValidator = passwordPolicyValidator;
    }

    public async Task Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Convert primitive data to Value Objects
        var userId = new AccountId(command.UserId);
        var email = new AccountEmail(command.Email);
        var username = new AccountUsername(command.Username);
        var name = new AccountName(command.Name);

        await ValidateUniqueness(email, username, cancellationToken);

        ValidatePasswordPolicy(command.PlainPassword);

        var passwordHash = GeneratePasswordHash(command.PlainPassword);
        
        var user = CreateUserInstance(userId, email, username, name, passwordHash);

        // Persist user
        await _userRepository.AddAsync(user, cancellationToken);
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
    
    private User CreateUserInstance(
        AccountId userId,
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        string passwordHash)
    {
        return new User(userId, email, username, name, passwordHash);
    }
}