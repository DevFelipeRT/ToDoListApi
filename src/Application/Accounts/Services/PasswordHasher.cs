using System;
using System.Security.Cryptography;
using Domain.Accounts.Services.Interfaces;

namespace Application.Accounts.Services;

/// <summary>
/// Provides secure password hashing and verification using PBKDF2 (Rfc2898DeriveBytes),
/// in accordance with Microsoft security recommendations for password storage.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32;  // 256 bits
    private const int Iterations = 100_000; // Recommended minimum: 100,000

    /// <summary>
    /// Hashes a plain text password using PBKDF2 with a unique, randomly generated salt.
    /// </summary>
    /// <param name="plainPassword">The user's plain text password.</param>
    /// <returns>
    /// A string containing the salt, the hash, and the iteration count,
    /// formatted as [salt]:[hash]:[iterations] (all Base64 encoded).
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the password is null or empty.</exception>
    public string HashPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be null or empty.", nameof(plainPassword));

        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            plainPassword, salt, Iterations, HashAlgorithmName.SHA256);

        var hash = pbkdf2.GetBytes(KeySize);

        // Output format: [salt]:[hash]:[iterations]
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}:{Iterations}";
    }

    /// <summary>
    /// Verifies a provided password against a previously hashed password.
    /// </summary>
    /// <param name="hashedPassword">
    /// The hashed password, in the format produced by <see cref="HashPassword"/>.
    /// </param>
    /// <param name="providedPassword">The plain text password to verify.</param>
    /// <returns>True if the password matches the hash; otherwise, false.</returns>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(providedPassword) ||
            string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        var parts = hashedPassword.Split(':');
        if (parts.Length != 3)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);
        if (!int.TryParse(parts[2], out var iterations))
            return false;

        using var pbkdf2 = new Rfc2898DeriveBytes(
            providedPassword, salt, iterations, HashAlgorithmName.SHA256);

        var computedHash = pbkdf2.GetBytes(KeySize);

        // Use a constant-time comparison to prevent timing attacks
        return CryptographicOperations.FixedTimeEquals(computedHash, hash);
    }
}
