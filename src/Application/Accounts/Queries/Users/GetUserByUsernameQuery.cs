namespace Application.Accounts.Queries.Users;

/// <summary>
/// Query representing the intention to retrieve a user by username.
/// </summary>
public sealed class GetUserByUsernameQuery
{
    public string Username { get; }

    public GetUserByUsernameQuery(string username)
    {
        Username = username;
    }
}
