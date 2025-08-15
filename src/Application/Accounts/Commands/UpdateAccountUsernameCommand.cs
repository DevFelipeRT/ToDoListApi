using MediatR;

namespace Application.Accounts.Commands;

/// <summary>
/// Command representing the intention to update an account's username.
/// </summary>
public sealed class UpdateAccountUsernameCommand : IRequest
{
    public Guid AccountId { get; }
    public string NewUsername { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAccountUsernameCommand"/> class.
    /// </summary>
    public UpdateAccountUsernameCommand(Guid accountId, string newUsername)
    {
        AccountId = accountId;
        NewUsername = newUsername;
    }
}
