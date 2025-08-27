using Application.Abstractions.Links;

namespace Application.IdentityAccess.Links;

/// <summary>
/// Options for account activation link composition.
/// </summary>
public sealed class ActivationLinkOptions : LinkOptions
{
    public string TokenKey { get; init; } = "token";
    public string IdKey    { get; init; } = "uid";
}
