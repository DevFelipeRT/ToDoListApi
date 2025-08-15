namespace Domain.Accounts.Policies.Interfaces;

/// <summary>
/// Service that validates password complexity policies.
/// </summary>
public interface IPasswordPolicy
{
    bool IsValid(string password, out string? reason);
}
