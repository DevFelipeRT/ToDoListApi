using MediatR;

namespace Application.Accounts.Commands.Users;

/// <summary>
/// Command representing the intention to update a user's username.
/// </summary>
public sealed class UpdateUserUsernameCommand : IRequest
{
    public Guid UserId { get; }
    public string NewUsername { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserUsernameCommand"/> class.
    /// </summary>
    public UpdateUserUsernameCommand(Guid userId, string newUsername)
    {
        UserId = userId;
        NewUsername = newUsername;
    }
}
