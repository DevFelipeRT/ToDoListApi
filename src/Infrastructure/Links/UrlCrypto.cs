using System;
using Application.Abstractions.Links;
using Microsoft.AspNetCore.DataProtection;

namespace Infrastructure.Links;

/// <summary>
/// Data Protection–based implementation of <see cref="IUrlCrypto"/>.
/// Produces Base64Url strings safe for inclusion in query strings.
/// </summary>
public sealed class UrlCrypto : IUrlCrypto
{
    private readonly IDataProtector _protector;

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlCrypto"/> class.
    /// </summary>
    /// <param name="provider">The Data Protection provider configured in the host application.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is null.</exception>
    public UrlCrypto(IDataProtectionProvider provider)
    {
        if (provider is null) throw new ArgumentNullException(nameof(provider));
        // Purpose string ensures cryptographic isolation for this use case.
        _protector = provider.CreateProtector("links:activation:uid:v1");
    }

    /// <inheritdoc />
    public string Protect(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
            throw new ArgumentException("Plaintext must be provided.", nameof(plaintext));

        return _protector.Protect(plaintext);
    }

    /// <inheritdoc />
    public string Unprotect(string protectedText)
    {
        if (string.IsNullOrWhiteSpace(protectedText))
            throw new ArgumentException("Protected text must be provided.", nameof(protectedText));

        return _protector.Unprotect(protectedText);
    }
}
