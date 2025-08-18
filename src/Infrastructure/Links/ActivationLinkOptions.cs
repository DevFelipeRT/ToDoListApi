namespace Infrastructure.Links;

/// <summary>
/// Options used to compose activation links.
/// </summary>
public sealed class ActivationLinkOptions
{
    /// <summary>
    /// Absolute base URL of the application handling the activation flow.
    /// Example: "https://app.example.com"
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Relative path segment for the activation endpoint.
    /// Example: "activate". Defaults to "activate" when not provided.
    /// </summary>
    public string Path { get; set; } = "activate";

    /// <summary>
    /// Query-string key used to carry the activation token.
    /// Example: "token". Defaults to "token" when not provided.
    /// </summary>
    public string QueryKey { get; set; } = "token";
}
