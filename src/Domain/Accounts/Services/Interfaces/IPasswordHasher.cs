namespace Domain.Accounts.Services.Interfaces;

/// <summary>
/// Service that hashes and verifies user passwords securely.
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string plainPassword);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}
