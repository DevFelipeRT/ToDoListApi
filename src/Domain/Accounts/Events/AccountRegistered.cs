using System;
using Domain.Accounts.ValueObjects;
using Domain.Abstractions.Events;

namespace Domain.Accounts.Events;

/// <summary>
/// Domain event raised after an account has been successfully registered (committed).
/// Carries only the identity; handlers can load additional data if needed.
/// </summary>
public sealed record AccountRegistered(
    AccountId AccountId,
    DateTimeOffset OccurredOn
) : IDomainEvent;
