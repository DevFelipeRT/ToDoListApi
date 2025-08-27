using System;
using System.Text;
using Application.Abstractions.Links;

namespace Infrastructure.Links;

/// <summary>
/// Default implementation of <see cref="ILinkBuilder"/> that composes absolute URLs.
/// Stateless; can be registered as a singleton.
/// </summary>
public sealed class LinkBuilder : ILinkBuilder
{
    public string Build(LinkOptions options, IReadOnlyDictionary<string, string>? queryParams = null)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new InvalidOperationException("LinkOptions.BaseUrl is required.");

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
            throw new InvalidOperationException("LinkOptions.BaseUrl must be an absolute URI.");

        if (!string.Equals(baseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(baseUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("LinkOptions.BaseUrl must use http or https.");
        }

        var pathSegment = NormalizeSegment(options.Path);
        var ub = new UriBuilder(baseUri)
        {
            Path = CombinePaths(baseUri.AbsolutePath, pathSegment)
        };

        // Merge existing base query, default query, and provided queryParams
        var merged = new Dictionary<string, string>(StringComparer.Ordinal);

        // Start from any existing query in BaseUrl
        var existing = ub.Query;
        if (!string.IsNullOrEmpty(existing) && existing[0] == '?')
            existing = existing.Substring(1);

        if (!string.IsNullOrWhiteSpace(existing))
        {
            foreach (var kv in existing.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = kv.Split('=', 2);
                var k = Uri.UnescapeDataString(parts[0]);
                var v = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
                merged[k] = v;
            }
        }

        // Default query from options
        foreach (var kv in options.DefaultQuery)
            merged[kv.Key] = kv.Value;

        // Provided query
        if (queryParams is not null)
        {
            foreach (var kv in queryParams)
                merged[kv.Key] = kv.Value;
        }

        // Rebuild query string (escaping)
        var sb = new StringBuilder();
        foreach (var kv in merged)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append(Uri.EscapeDataString(kv.Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(kv.Value ?? string.Empty));
        }

        ub.Query = sb.ToString();
        return ub.Uri.AbsoluteUri;
    }

    private static string NormalizeSegment(string? segment)
    {
        if (string.IsNullOrWhiteSpace(segment)) return string.Empty;
        return segment.Trim().Trim('/');
    }

    private static string CombinePaths(string basePath, string segment)
    {
        var a = (basePath ?? string.Empty).TrimEnd('/');
        var b = (segment ?? string.Empty).Trim('/');

        if (string.IsNullOrEmpty(a) || a == "/") return string.IsNullOrEmpty(b) ? "/" : "/" + b;
        if (string.IsNullOrEmpty(b)) return a.Length == 0 ? "/" : a;
        return $"{a}/{b}";
    }
}
