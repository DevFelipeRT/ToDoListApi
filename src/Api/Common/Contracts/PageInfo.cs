namespace Api.Common.Contracts;

/// <summary>
/// Pagination metadata for paged API responses.
/// </summary>
public class PageInfo
{
    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages available.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Total number of items available.
    /// </summary>
    public int TotalItems { get; set; }
}