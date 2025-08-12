using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Repositories;
using Application.Accounts.DTOs;
using Domain.Accounts;
using Domain.Accounts.ValueObjects;

namespace Application.Accounts.Queries.Users.Handlers;

/// <summary>
/// Handler responsible for retrieving a user by email from the repository and converting to DTO.
/// </summary>
public sealed class GetUserByEmailHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Handles the query to retrieve a user by email.
    /// </summary>
    /// <param name="query">The query containing the user email.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The UserDto if found; otherwise, null.</returns>
    public async Task<UserDto?> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        // Convert primitive to Value Object
        var email = new AccountEmail(query.Email);

        var user = await RetrieveUserByEmail(email, cancellationToken);

        if (user == null)
            return null;

        return UserDto.FromAggregate(user);
    }

    private async Task<User?> RetrieveUserByEmail(AccountEmail email, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
            return null;
        return user;
    }
}
