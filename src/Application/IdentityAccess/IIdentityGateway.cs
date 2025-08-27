using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;

namespace Application.IdentityAccess;

/// <summary>
/// Outbound port for neutral user lifecycle operations against an external IAM provider.
/// Token-based flows (activation, password reset) are handled by dedicated services.
/// </summary>
public interface IIdentityGateway
{
    /// <summary>
    /// Creates a new user in the external identity provider and returns its credential identifier.
    /// </summary>
    Task<CredentialId> CreateUserAsync(
        string email,
        string username,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user from the external identity provider.
    /// </summary>
    Task DeleteUserAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user's credential identifier by email if present.
    /// </summary>
    Task<CredentialId?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user's credential identifier by username if present.
    /// </summary>
    Task<CredentialId?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves user information from the external identity provider by credential identifier.
    /// </summary>
    Task<IdentityUserDto?> FindByCredentialIdAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indicates whether the user's email has been confirmed in the external provider.
    /// </summary>
    Task<bool> IsEmailConfirmedAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the user's password by validating the current password.
    /// </summary>
    Task ChangePasswordAsync(
        CredentialId credentialId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}

