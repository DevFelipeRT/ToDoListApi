using Application.Abstractions.Links;

namespace Application.IdentityAccess.Links;

/// <summary>
/// Options for password reset link composition.
/// </summary>
public sealed class ResetPasswordLinkOptions : LinkOptions
{
    public string TokenKey { get; init; } = "resetToken";
    public string IdKey    { get; init; } = "uid";
}