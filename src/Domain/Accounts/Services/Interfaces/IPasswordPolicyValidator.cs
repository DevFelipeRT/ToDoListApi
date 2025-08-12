namespace Domain.Accounts.Services.Interfaces;

/// <summary>
/// Service that validates password complexity policies.
/// </summary>
public interface IPasswordPolicyValidator
{
    bool IsValid(string password, out string? reason);
}
