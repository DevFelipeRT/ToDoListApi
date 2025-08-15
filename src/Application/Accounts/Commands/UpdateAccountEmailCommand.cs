using MediatR;

namespace Application.Accounts.Commands;

/// <summary>
/// Command representing the intention to update an account's email address.
/// </summary>
public sealed class UpdateAccountEmailCommand : IRequest
{
    public Guid AccountId { get; }
    public string NewEmail { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAccountEmailCommand"/> class.
    /// </summary>
    public UpdateAccountEmailCommand(Guid accountId, string newEmail)
    {
        AccountId = accountId;
        NewEmail = newEmail;
    }
}
