namespace ToDoApi.Common.Contracts;

/// <summary>
/// Parameters for paginated API requests.
/// </summary>
public class PaginationParameters
{
    /// <summary>
    /// Page number to retrieve (1-based).
    /// </summary>
    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 1 : value;
    }
}
