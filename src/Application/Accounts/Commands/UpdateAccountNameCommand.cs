using MediatR;

namespace Application.Accounts.Commands;
/// <summary>
/// Command representing the intention to update an account's display name.
/// </summary>
public sealed class UpdateAccountNameCommand : IRequest
{
    public Guid AccountId { get; }
    public string NewName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAccountNameCommand"/> class.
    /// </summary>
    public UpdateAccountNameCommand(Guid accountId, string newName)
    {
        AccountId = accountId;
        NewName = newName;
    }
}
