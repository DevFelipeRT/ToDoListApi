namespace Application.Abstractions.Links;

/// <summary>
/// Provides URL-safe encryption and decryption for link parameters.
/// Typically backed by ASP.NET Core Data Protection.
/// </summary>
public interface IUrlCrypto
{
    /// <summary>
    /// Protects (encrypts and signs) a plain text value into a URL-safe representation.
    /// </summary>
    /// <param name="plaintext">The value to protect.</param>
    /// <returns>A protected string safe to include in URLs.</returns>
    string Protect(string plaintext);

    /// <summary>
    /// Unprotects (decrypts and validates) a previously protected value.
    /// </summary>
    /// <param name="protectedText">The protected value to unprotect.</param>
    /// <returns>The original plain text.</returns>
    string Unprotect(string protectedText);
}
