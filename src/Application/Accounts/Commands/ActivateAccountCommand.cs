using MediatR;

namespace Application.Accounts.Commands;

/// <summary>
/// Command that encapsulates the data required to activate an account.
/// This command is specific to concrete accounts.
/// </summary>
public sealed class ActivateAccountCommand : IRequest
{
    /// <summary>
    /// Gets the unique identifier of the account (raw Guid).
    /// </summary>
    public Guid AccountId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivateAccountCommand"/> class.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    public ActivateAccountCommand(Guid accountId)
    {
        AccountId = accountId;
    }
}
