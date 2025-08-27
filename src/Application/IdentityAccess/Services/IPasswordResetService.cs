using System;
using System.Threading;
using System.Threading.Tasks;
using Application.IdentityAccess.Contracts;
using Domain.Accounts.ValueObjects;

namespace Application.IdentityAccess.Services;

/// <summary>
/// Application service contract for password reset flow.
/// </summary>
public interface IPasswordResetService
{
    /// <summary>
    /// Generates a password reset token for the given account.
    /// </summary>
    Task<PasswordResetToken> GenerateResetTokenAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the user's password using the provided token.
    /// </summary>
    Task<bool> ResetPasswordAsync(
        CredentialId credentialId,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);
}

