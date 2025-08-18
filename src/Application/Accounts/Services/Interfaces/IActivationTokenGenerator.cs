using System;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Entities;

namespace Application.Accounts.Services;

/// <summary>
/// Contract for issuing, verifying, and consuming account activation tokens
/// in alignment with domain semantics. No persistence or transport concerns.
/// </summary>
public interface IActivationTokenService
{
    /// <summary>
    /// Issues a new activation token for the given account.
    /// Returns the domain entity (to be persisted) and the raw token (to be sent to the user and NEVER persisted).
    /// </summary>
    /// <param name="userId">Owning account identifier.</param>
    /// <param name="now">Clock reference for issuance.</param>
    /// <param name="timeToLive">Token lifetime; must be greater than zero.</param>
    /// <returns>(Token, RawToken)</returns>
    (ActivationToken Token, string RawToken) Issue(AccountId userId, DateTimeOffset now, TimeSpan timeToLive);

    /// <summary>
    /// Verifies whether the presented raw token matches the token's stored hash using constant-time comparison.
    /// </summary>
    /// <param name="token">Persisted activation token entity.</param>
    /// <param name="rawToken">Raw token presented by the user.</param>
    /// <returns>True if the hashes match; otherwise false.</returns>
    bool VerifyRaw(ActivationToken token, string rawToken);

    /// <summary>
    /// Verifies a raw token against a stored uppercase hex hash using constant-time comparison.
    /// </summary>
    /// <param name="rawToken">Raw token presented by the user.</param>
    /// <param name="storedHashHex">Uppercase hex representation of the stored hash.</param>
    /// <returns>True if the hashes match; otherwise false.</returns>
    bool VerifyRaw(string rawToken, string storedHashHex);

    /// <summary>
    /// Attempts to verify and consume the token atomically (single-use) at the given instant.
    /// Returns true when the token is active, the hash matches, and consumption succeeds.
    /// </summary>
    /// <param name="token">Persisted activation token entity.</param>
    /// <param name="rawToken">Raw token presented by the user.</param>
    /// <param name="when">Clock reference used for validation and stamping consumption.</param>
    /// <returns>True if the token transitioned to consumed; otherwise false.</returns>
    bool VerifyAndConsume(ActivationToken token, string rawToken, DateTimeOffset when);

    /// <summary>
    /// Generates a cryptographically strong random token encoded as Base64Url without padding.
    /// </summary>
    /// <param name="numBytes">Number of random bytes; defaults to 32 (256 bits).</param>
    /// <returns>The raw token string.</returns>
    string GenerateRawToken(int numBytes = 32);

    /// <summary>
    /// Computes the SHA-256 hash of a raw token as an uppercase hexadecimal string.
    /// </summary>
    /// <param name="raw">Raw token string.</param>
    /// <returns>Uppercase hex hash.</returns>
    string ComputeHash(string raw);

    /// <summary>
    /// Convenience that returns a freshly generated raw token and its SHA-256 hash.
    /// Never persist the raw value.
    /// </summary>
    /// <returns>(RawToken, HashedToken)</returns>
    (string RawToken, string HashedToken) CreateMaterials();
}
