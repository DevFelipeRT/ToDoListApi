using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Repositories;
using Application.Accounts.DTOs;
using Domain.Accounts.ValueObjects;
using Domain.Accounts;

namespace Application.Accounts.Queries.Users.Handlers;

/// <summary>
/// Handler responsible for retrieving a user by username from the repository and converting to DTO.
/// </summary>
public sealed class GetUserByUsernameHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByUsernameHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Handles the query to retrieve a user by username.
    /// </summary>
    /// <param name="query">The query containing the username.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The UserDto if found; otherwise, null.</returns>
    public async Task<UserDto?> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var username = new AccountUsername(query.Username);

        var user = await RetrieveUser(username, cancellationToken);

        return UserDto.FromAggregate(user);
    }

    private async Task<User> RetrieveUser(AccountUsername username, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user is null)
            throw new InvalidOperationException("User not found.");
        return user;
    }
}
