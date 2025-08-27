using System;
using System.Threading;
using System.Threading.Tasks;
using Application.IdentityAccess.Contracts;
using Domain.Accounts.ValueObjects;

namespace Application.IdentityAccess.Services;

/// <summary>
/// Contract for managing account activation via email confirmation.
/// </summary>
public interface IEmailActivationService
{
    /// <summary>
    /// Generates an email activation token for the given account.
    /// </summary>
    Task<ActivationToken> GenerateEmailActivationTokenAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms account activation using a previously issued email activation token.
    /// </summary>
    Task<bool> ConfirmEmailActivationAsync(
        CredentialId credentialId,
        string token,
        CancellationToken cancellationToken = default);
}
