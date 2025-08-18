using Api.Common.Contracts;

namespace Api.ListManagement.Contracts.Responses;

/// <summary>
/// Paginated response model for todo lists.
/// </summary>
public class PagedListResponse
{
    /// <summary>
    /// Collection of todo lists in the current page.
    /// </summary>
    public IEnumerable<ListResponse> Items { get; set; } = new List<ListResponse>();

    /// <summary>
    /// Pagination metadata.
    /// </summary>
    public PageInfo Pagination { get; set; } = new();

    /// <summary>
    /// HATEOAS links for pagination navigation.
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
