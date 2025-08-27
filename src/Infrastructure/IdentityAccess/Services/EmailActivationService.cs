using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.IdentityAccess.Services;
using Application.IdentityAccess.Contracts;
using Domain.Accounts.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Infrastructure.IdentityAccess.Services;

/// <inheritdoc cref="IEmailActivationService" />
public sealed class EmailActivationService : IEmailActivationService
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly IOptions<DataProtectionTokenProviderOptions> _tokenOptions;

    public EmailActivationService(
        UserManager<ApplicationUser> users,
        IOptions<DataProtectionTokenProviderOptions> tokenOptions)
    {
        _users = users ?? throw new ArgumentNullException(nameof(users));
        _tokenOptions = tokenOptions ?? throw new ArgumentNullException(nameof(tokenOptions));
    }

    /// <inheritdoc />
    public async Task<ActivationToken> GenerateEmailActivationTokenAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(credentialId);

        var token = await _users.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var lifespan = _tokenOptions.Value?.TokenLifespan;

        var expiresAt = lifespan.HasValue
            ? DateTimeOffset.UtcNow.Add(lifespan.Value)
            : (DateTimeOffset?)null;

        return new ActivationToken(encodedToken, expiresAt);
    }

    /// <inheritdoc />
    public async Task<bool> ConfirmEmailActivationAsync(
        CredentialId credentialId,
        string token,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch
        {
            return false;
        }

        var user = await RequireUserAsync(credentialId);

        var result = await _users.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded;
    }

    private async Task<ApplicationUser> RequireUserAsync(CredentialId credentialId)
    {
        var id = credentialId.Value;

        var user = await _users.FindByIdAsync(id);

        if (user is null)
            throw new InvalidOperationException("User not found for the provided credential identifier.");

        return user;
    }
}
