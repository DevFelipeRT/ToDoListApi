using MediatR;

namespace Application.Accounts.Commands.Users;

/// <summary>
/// Command representing the intention to update a user's email address.
/// </summary>
public sealed class UpdateUserEmailCommand : IRequest
{
    public Guid UserId { get; }
    public string NewEmail { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserEmailCommand"/> class.
    /// </summary>
    public UpdateUserEmailCommand(Guid userId, string newEmail)
    {
        UserId = userId;
        NewEmail = newEmail;
    }
}
