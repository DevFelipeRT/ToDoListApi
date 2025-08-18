namespace Api.Common.Contracts;

/// <summary>
/// Hypermedia link for HATEOAS responses.
/// </summary>
public class Link
{
    /// <summary>
    /// The URL of the link.
    /// </summary>
    public string Href { get; set; } = string.Empty;

    /// <summary>
    /// The relation type of the link (e.g., self, next, previous).
    /// </summary>
    public string Rel { get; set; } = string.Empty;

    /// <summary>
    /// The HTTP method for the link (e.g., GET, POST).
    /// </summary>
    public string Method { get; set; } = string.Empty;
}

