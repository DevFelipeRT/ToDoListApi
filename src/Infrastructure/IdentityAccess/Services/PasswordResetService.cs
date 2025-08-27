using System;
using System.Threading;
using System.Threading.Tasks;
using Application.IdentityAccess.Services;
using Application.IdentityAccess.Contracts;
using Domain.Accounts.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.IdentityAccess.Services;

/// <inheritdoc cref="IPasswordResetService" />
public sealed class PasswordResetService : IPasswordResetService
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly IOptions<DataProtectionTokenProviderOptions> _tokenOptions;

    public PasswordResetService(
        UserManager<ApplicationUser> users,
        IOptions<DataProtectionTokenProviderOptions> tokenOptions)
    {
        _users = users ?? throw new ArgumentNullException(nameof(users));
        _tokenOptions = tokenOptions ?? throw new ArgumentNullException(nameof(tokenOptions));
    }

    /// <inheritdoc />
    public async Task<PasswordResetToken> GenerateResetTokenAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(credentialId);
        var token = await _users.GeneratePasswordResetTokenAsync(user);

        var lifespan = _tokenOptions.Value?.TokenLifespan;
        var expiresAt = lifespan.HasValue
            ? DateTimeOffset.UtcNow.Add(lifespan.Value)
            : (DateTimeOffset?)null;

        return new PasswordResetToken(token, expiresAt);
    }

    /// <inheritdoc />
    public async Task<bool> ResetPasswordAsync(
        CredentialId credentialId,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            return false;

        var user = await RequireUserAsync(credentialId);
        var result = await _users.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    private async Task<ApplicationUser> RequireUserAsync(CredentialId credentialId)
    {
        var id = credentialId.TryGetGuid(out var guid) ? guid.ToString("D") : credentialId.Value;
        var user = await _users.FindByIdAsync(id);
        if (user is null)
            throw new InvalidOperationException("User not found for the provided credential identifier.");
        return user;
    }
}
