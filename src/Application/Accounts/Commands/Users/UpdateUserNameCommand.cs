using MediatR;

namespace Application.Accounts.Commands.Users;

/// <summary>
/// Command representing the intention to update a user's display name.
/// </summary>
public sealed class UpdateUserNameCommand : IRequest
{
    public Guid UserId { get; }
    public string NewName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserNameCommand"/> class.
    /// </summary>
    public UpdateUserNameCommand(Guid userId, string newName)
    {
        UserId = userId;
        NewName = newName;
    }
}
