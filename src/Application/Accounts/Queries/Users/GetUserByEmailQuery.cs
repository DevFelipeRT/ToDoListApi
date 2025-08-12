namespace Application.Accounts.Queries.Users;

/// <summary>
/// Query representing the intention to retrieve a user by email address.
/// </summary>
public sealed class GetUserByEmailQuery
{
    public string Email { get; }

    public GetUserByEmailQuery(string email)
    {
        Email = email;
    }
}
