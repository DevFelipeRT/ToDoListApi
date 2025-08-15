using MediatR;

namespace Application.Accounts.Commands.Users;

/// <summary>
/// Command that encapsulates the data required to deactivate a User account.
/// This command is specific to concrete User accounts.
/// </summary>
public sealed class DeactivateUserCommand : IRequest
{
    public Guid UserId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateUserCommand"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user account.</param>
    public DeactivateUserCommand(Guid userId)
    {
        UserId = userId;
    }
}
