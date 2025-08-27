namespace Application.Abstractions.Links;

/// <summary>
/// Builds absolute URLs from options and query parameters.
/// Stateless and reusable across different link scenarios.
/// </summary>
public interface ILinkBuilder
{
    /// <summary>
    /// Builds an absolute URL by combining base url, path and query parameters.
    /// </summary>
    string Build(LinkOptions options, IReadOnlyDictionary<string, string>? queryParams = null);
}

