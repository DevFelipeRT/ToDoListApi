using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.IdentityAccess;
using Domain.Accounts.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.IdentityAccess.Services;

/// <summary>
/// Identity gateway backed by ASP.NET Core Identity for neutral user lifecycle operations.
/// </summary>
public sealed class IdentityService : IIdentityGateway
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    /// <inheritdoc />
    public async Task<CredentialId> CreateUserAsync(
        string email,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser(Guid.NewGuid(), username, email, username);
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));

        return CredentialId.FromGuid(user.Id);
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(credentialId);
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
    }

    /// <inheritdoc />
    public async Task<CredentialId?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? null : CredentialId.FromGuid(user.Id);
    }

    /// <inheritdoc />
    public async Task<CredentialId?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user is null ? null : CredentialId.FromGuid(user.Id);
    }

    /// <inheritdoc />
    public async Task<IdentityUserDto?> FindByCredentialIdAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = credentialId.TryGetGuid(out var guid)
            ? guid.ToString("D")
            : credentialId.Value;

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return null;

        return new IdentityUserDto(
            email: user.Email ?? string.Empty,
            username: user.UserName ?? string.Empty,
            emailConfirmed: user.EmailConfirmed
        );
    }


    /// <inheritdoc />
    public async Task<bool> IsEmailConfirmedAsync(
        CredentialId credentialId,
        CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(credentialId);
        return await _userManager.IsEmailConfirmedAsync(user);
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(
        CredentialId credentialId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(credentialId);
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
    }

    private async Task<ApplicationUser> RequireUserAsync(CredentialId credentialId)
    {
        var id = credentialId.TryGetGuid(out var guid) ? guid.ToString("D") : credentialId.Value;
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            throw new InvalidOperationException("User not found for the provided credential identifier.");
        return user;
    }
}
