using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Repositories;
using Application.Accounts.DTOs;
using Domain.Accounts.ValueObjects;
using Domain.Accounts;

namespace Application.Accounts.Queries.Users.Handlers;

/// <summary>
/// Handler responsible for searching users with optional filters and pagination.
/// </summary>
public sealed class SearchUsersHandler
{
    private readonly IUserRepository _userRepository;

    public SearchUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Handles the search query and returns a paginated list of UserDto.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of UserDto matching the criteria.</returns>
    public async Task<IReadOnlyCollection<UserDto>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var name = query.Name != null ? new AccountName(query.Name) : null;
        var username = query.Username != null ? new AccountUsername(query.Username) : null;
        var email = query.Email != null ? new AccountEmail(query.Email) : null;
        var isActive = query.IsActive.HasValue ? (bool)query.IsActive : (bool?)null;

        var users = await SearchAsync(
            name,
            username,
            email,
            isActive,
            query.Page,
            query.PageSize,
            cancellationToken);

        var dtos = ConvertToDtos(users);

        return dtos;
    }

    private async Task<IReadOnlyCollection<User>> SearchAsync(
        AccountName? name,
        AccountUsername? username,
        AccountEmail? email,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await _userRepository.SearchAsync(name, username, email, isActive, page, pageSize, cancellationToken);
    }

    private IReadOnlyCollection<UserDto> ConvertToDtos(IReadOnlyCollection<User> users)
    {
        var dtos = new List<UserDto>();

        foreach (var user in users)
        {
            dtos.Add(UserDto.FromAggregate(user));
        }
        
        return dtos;
    }
}
