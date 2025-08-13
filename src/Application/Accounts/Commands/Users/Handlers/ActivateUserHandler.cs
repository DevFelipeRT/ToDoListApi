using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using Domain.Accounts;

namespace Application.Accounts.Commands.Users.Handlers;

/// <summary>
/// Handler responsible for activating a user account.
/// </summary>
public sealed class ActivateUserHandler : IRequestHandler<ActivateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public ActivateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(ActivateUserCommand command, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var userId = AccountId.FromGuid(command.UserId);

        var user = RetrieveUser(userId, cancellationToken);
        
        user.Activate();

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
}
