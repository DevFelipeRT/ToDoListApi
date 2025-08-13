using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Repositories;
using Application.Accounts.DTOs;
using Domain.Accounts.ValueObjects;
using Domain.Accounts;

namespace Application.Accounts.Queries.Users.Handlers;

/// <summary>
/// Handler responsible for retrieving a user by id from the repository and converting to DTO.
/// </summary>
public sealed class GetUserByIdHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Handles the query to retrieve a user by id.
    /// </summary>
    /// <param name="query">The query containing the user id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The UserDto if found; otherwise, null.</returns>
    public async Task<UserDto?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var userId = AccountId.FromGuid(query.UserId);

        var user = await RetrieveUser(userId, cancellationToken);

        return UserDto.FromAggregate(user);
    }

    private async Task<User> RetrieveUser(AccountId userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            throw new InvalidOperationException("User not found.");
        return user;
    }
}
