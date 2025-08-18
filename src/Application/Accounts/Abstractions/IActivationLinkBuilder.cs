using System;

namespace Application.Accounts.Abstractions;

/// <summary>
/// Builds a deterministic, absolute activation URL given an opaque token.
/// </summary>
/// <remarks>
/// This is an application-layer port. Concrete implementations live in the infrastructure layer.
/// The builder must be pure (no side effects) and thread-safe.
/// </remarks>
public interface IActivationLinkBuilder
{
    /// <summary>
    /// Produces a fully-qualified activation link that clients can open to complete account activation.
    /// </summary>
    /// <param name="token">Opaque activation token issued by the backend.</param>
    /// <returns>An absolute activation URL.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="token"/> is null or whitespace.</exception>
    string Build(string token);
}
