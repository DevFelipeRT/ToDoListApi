using System;
using MediatR;

namespace Application.Accounts.Commands;

/// <summary>
/// Requests re-issuance of an activation token and dispatch of the activation e-mail
/// for the account associated with the provided e-mail address.
/// </summary>
public sealed class ResendActivationCommand : IRequest
{
    /// <summary>The target account e-mail address (raw input, trimmed).</summary>
    public string Email { get; }

    public ResendActivationCommand(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail is required.", nameof(email));

        Email = email.Trim();
    }
}
