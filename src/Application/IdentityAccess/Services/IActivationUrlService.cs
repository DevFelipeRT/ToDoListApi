using Domain.Accounts.ValueObjects;
using Application.IdentityAccess.Contracts;

namespace Application.IdentityAccess.Services;

/// <summary>
/// Composes an activation URL for email confirmation by protecting the credential identifier
/// and delegating URL composition to a generic link builder.
/// </summary>
public interface IActivationUrlService
{
    /// <summary>
    /// Builds an absolute activation URL carrying the provided activation token and a protected credential identifier.
    /// </summary>
    string BuildActivationUrl(ActivationToken token, CredentialId credentialId);
}
