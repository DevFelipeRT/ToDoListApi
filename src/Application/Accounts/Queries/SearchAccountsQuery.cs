using MediatR;
using System.Collections.Generic;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries;

/// <summary>
/// Query representing the intention to search accounts by optional filters and pagination.
/// </summary>
public sealed class SearchAccountsQuery : IRequest<IReadOnlyCollection<AccountDto>>
{
    public string? Name { get; }
    public string? Username { get; }
    public string? Email { get; }
    public bool? IsActive { get; }
    public int Page { get; }
    public int PageSize { get; }

    public SearchAccountsQuery(
        string? name = null,
        string? username = null,
        string? email = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20)
    {
        Name = name;
        Username = username;
        Email = email;
        IsActive = isActive;
        Page = page > 0 ? page : 1;
        PageSize = pageSize > 0 ? pageSize : 20;
    }
}
