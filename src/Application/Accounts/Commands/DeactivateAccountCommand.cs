using MediatR;

namespace Application.Accounts.Commands;

/// <summary>
/// Command that encapsulates the data required to deactivate an account.
/// This command is specific to concrete accounts.
/// </summary>
public sealed class DeactivateAccountCommand : IRequest
{
    public Guid AccountId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateAccountCommand"/> class.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    public DeactivateAccountCommand(Guid accountId)
    {
        AccountId = accountId;
    }
}
