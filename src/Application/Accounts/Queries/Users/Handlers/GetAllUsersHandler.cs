using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Repositories;
using Application.Accounts.DTOs;
using Domain.Accounts;

namespace Application.Accounts.Queries.Users.Handlers;

/// <summary>
/// Handler responsible for retrieving all users from the repository and converting them to DTOs.
/// </summary>
public sealed class GetAllUsersHandler
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Handles the query to retrieve all users.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of UserDto representing all users.</returns>
    public async Task<IReadOnlyCollection<UserDto>> Handle(CancellationToken cancellationToken)
    {
        var users = await RetrieveAllUsers(cancellationToken);

        return ConvertToDto(users);
    }

    private async Task<IReadOnlyCollection<User>> RetrieveAllUsers(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        if (users == null || users.Count == 0)
            throw new InvalidOperationException("No users found.");
        return users;
    }

    private IReadOnlyCollection<UserDto> ConvertToDto(IReadOnlyCollection<User> users)
    {
        var dtos = new List<UserDto>();
        foreach (var user in users)
            dtos.Add(UserDto.FromAggregate(user));
        return dtos;
    }
}
