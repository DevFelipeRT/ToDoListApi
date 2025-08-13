using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.Services.Interfaces;
using Domain.Accounts.ValueObjects;
using Domain.Accounts;

namespace Application.Accounts.Commands.Users.Handlers;

/// <summary>
/// Handler responsible for updating a user's username.
/// </summary>
public sealed class UpdateUserUsernameHandler : IRequestHandler<UpdateUserUsernameCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountUniquenessChecker _uniquenessChecker;

    public UpdateUserUsernameHandler(
        IUserRepository userRepository,
        IAccountUniquenessChecker uniquenessChecker)
    {
        _userRepository = userRepository;
        _uniquenessChecker = uniquenessChecker;
    }

    public async Task Handle(UpdateUserUsernameCommand command, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = AccountId.FromGuid(command.UserId);
        var newUsername = new AccountUsername(command.NewUsername);

        var user = RetrieveUser(userId, cancellationToken);

        await ValidateNewUsernameUniqueness(newUsername, cancellationToken);

        user.UpdateUsername(newUsername);

        // Persist changes
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    private User RetrieveUser(AccountId userId, CancellationToken cancellationToken)
    {
        var user = _userRepository.GetByIdAsync(userId, cancellationToken).Result;
        if (user == null)
            throw new InvalidOperationException("User not found.");
        return user;
    }
    
    private async Task ValidateNewUsernameUniqueness(AccountUsername username, CancellationToken cancellationToken)
    {
        if (!await _uniquenessChecker.IsUsernameUniqueAsync(username, cancellationToken))
            throw new InvalidOperationException("Username already in use.");
    }
}
