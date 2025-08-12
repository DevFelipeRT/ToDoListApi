using Domain.Accounts.Services.Interfaces;

namespace Domain.Accounts.Services;

/// <summary>
/// Implementation of password policy validation service.
/// </summary>
public sealed class PasswordPolicyValidator : IPasswordPolicyValidator
{
    private const int MinLength = 8;
    private const int MaxLength = 128;

    public bool IsValid(string password, out string? reason)
    {
        reason = null;

        if (string.IsNullOrWhiteSpace(password))
        {
            reason = "Password cannot be null or empty.";
            return false;
        }

        if (password.Length < MinLength)
        {
            reason = $"Password must be at least {MinLength} characters long.";
            return false;
        }

        if (password.Length > MaxLength)
        {
            reason = $"Password cannot exceed {MaxLength} characters.";
            return false;
        }

        bool hasUpper = false;
        bool hasLower = false;
        bool hasDigit = false;
        bool hasSpecialChar = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsLower(c)) hasLower = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsLetterOrDigit(c)) hasSpecialChar = true;
        }

        if (!hasUpper)
        {
            reason = "Password must contain at least one uppercase letter.";
            return false;
        }

        if (!hasLower)
        {
            reason = "Password must contain at least one lowercase letter.";
            return false;
        }

        if (!hasDigit)
        {
            reason = "Password must contain at least one digit.";
            return false;
        }

        if (!hasSpecialChar)
        {
            reason = "Password must contain at least one special character.";
            return false;
        }

        return true;
    }
}