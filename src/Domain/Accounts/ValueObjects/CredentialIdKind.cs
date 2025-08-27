using System;

namespace Domain.Accounts.ValueObjects;

/// <summary>
/// Discriminates the concrete format of a credential identifier.
/// Extend this enumeration to support additional formats as needed.
/// </summary>
public enum CredentialIdKind
{
    Guid = 1,
    String = 2
}

