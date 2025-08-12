using ToDoApi.Common.Contracts;

namespace ToDoApi.ListManagement.Contracts.Responses;

/// <summary>
/// Paginated response model for todo items.
/// </summary>
public class PagedToDoItemResponse
{
    /// <summary>
    /// Collection of todo items in the current page.
    /// </summary>
    public IEnumerable<ToDoItemResponse> Items { get; set; } = new List<ToDoItemResponse>();

    /// <summary>
    /// Pagination metadata.
    /// </summary>
    public PageInfo Pagination { get; set; } = new();

    /// <summary>
    /// HATEOAS links for pagination navigation.
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
