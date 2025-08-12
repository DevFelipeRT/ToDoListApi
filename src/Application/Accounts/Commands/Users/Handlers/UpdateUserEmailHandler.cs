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
/// Handler responsible for updating a user's email address.
/// </summary>
public sealed class UpdateUserEmailHandler : IRequestHandler<UpdateUserEmailCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountUniquenessChecker _uniquenessChecker;

    public UpdateUserEmailHandler(
        IUserRepository userRepository,
        IAccountUniquenessChecker uniquenessChecker)
    {
        _userRepository = userRepository;
        _uniquenessChecker = uniquenessChecker;
    }

    public async Task Handle(UpdateUserEmailCommand command, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = new AccountId(command.UserId);
        var newEmail = new AccountEmail(command.NewEmail);

        var user = RetrieveUser(userId, cancellationToken);

        await ValidateNewEmailUniqueness(newEmail, cancellationToken);
        
        user.UpdateEmail(newEmail);

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

    private async Task ValidateNewEmailUniqueness(AccountEmail email, CancellationToken cancellationToken)
    {
        if (!await _uniquenessChecker.IsEmailUniqueAsync(email, cancellationToken))
            throw new InvalidOperationException("Email already in use.");
    }
}
