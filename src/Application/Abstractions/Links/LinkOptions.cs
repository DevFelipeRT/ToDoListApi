namespace Application.Abstractions.Links
{
    /// <summary>
    /// Base abstract contract for link options.
    /// Specific link scenarios (activation, reset, etc.) extend this class with extra fields if needed.
    /// </summary>
    public abstract class LinkOptions
    {
        /// <summary>
        /// Absolute base URL (e.g., "https://app.example.com").
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Relative path segment (e.g., "activate" or "account/reset-password").
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Optional default query parameters applied to every link built with these options.
        /// </summary>
        public IDictionary<string, string> DefaultQuery { get; set; } = new Dictionary<string, string>();
    }
}
