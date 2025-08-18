using System;
using Application.Accounts.Abstractions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Links;

/// <summary>
/// Default activation link builder that composes an absolute URL using <see cref="ActivationLinkOptions"/>.
/// </summary>
/// <remarks>
/// Stateless and thread-safe; can be registered as a singleton.
/// </remarks>
public sealed class ActivationLinkBuilder : IActivationLinkBuilder
{
    private readonly ActivationLinkOptions _options;

    public ActivationLinkBuilder(IOptions<ActivationLinkOptions> options)
        => _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public string Build(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token must be provided.", nameof(token));

        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            throw new InvalidOperationException("ActivationLinkOptions.BaseUrl is required.");

        if (!Uri.TryCreate(_options.BaseUrl, UriKind.Absolute, out var baseUri))
            throw new InvalidOperationException("ActivationLinkOptions.BaseUrl must be an absolute URI.");

        if (!string.Equals(baseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(baseUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("ActivationLinkOptions.BaseUrl must use http or https.");
        }

        var pathSegment = NormalizeSegment(_options.Path, fallback: "activate");
        var queryKey = string.IsNullOrWhiteSpace(_options.QueryKey) ? "token" : _options.QueryKey.Trim();

        var ub = new UriBuilder(baseUri)
        {
            Path = CombinePaths(baseUri.AbsolutePath, pathSegment)
        };

        // Preserve any existing query from BaseUrl and append the token parameter.
        var existing = ub.Query;
        if (!string.IsNullOrEmpty(existing) && existing[0] == '?')
            existing = existing.Substring(1);

        var newParam = $"{queryKey}={Uri.EscapeDataString(token)}";
        ub.Query = string.IsNullOrEmpty(existing) ? newParam : $"{existing}&{newParam}";

        return ub.Uri.AbsoluteUri;
    }

    private static string NormalizeSegment(string? segment, string fallback)
    {
        var s = string.IsNullOrWhiteSpace(segment) ? fallback : segment;
        return s.Trim().Trim('/');
    }

    private static string CombinePaths(string basePath, string segment)
    {
        var a = (basePath ?? string.Empty).TrimEnd('/');
        var b = (segment ?? string.Empty).Trim('/');
        if (string.IsNullOrEmpty(a) || a == "/") return "/" + b;
        if (string.IsNullOrEmpty(b)) return a.Length == 0 ? "/" : a;
        return $"{a}/{b}";
    }
}
