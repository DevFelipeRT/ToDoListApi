using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using Domain.Accounts;

namespace Application.Accounts.Commands.Users.Handlers;

/// <summary>
/// Handler responsible for updating a user's display name.
/// </summary>
public sealed class UpdateUserNameHandler : IRequestHandler<UpdateUserNameCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserNameHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserNameCommand command, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = AccountId.FromGuid(command.UserId);
        var newName = new AccountName(command.NewName);

        var user = RetrieveUser(userId, cancellationToken);

        user.UpdateName(newName);

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
