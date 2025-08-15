using MediatR;

namespace Application.Accounts.Commands.Users;

/// <summary>
/// Command that encapsulates the data required to activate a User account.
/// This command is specific to concrete User accounts.
/// </summary>
public sealed class ActivateUserCommand : IRequest
{
    /// <summary>
    /// Gets the unique identifier of the user account (raw Guid).
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivateUserCommand"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user account.</param>
    public ActivateUserCommand(Guid userId)
    {
        UserId = userId;
    }
}
