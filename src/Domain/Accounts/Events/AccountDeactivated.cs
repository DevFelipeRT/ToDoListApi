using System;
using Domain.Accounts.ValueObjects;
using Domain.Abstractions.Events;

namespace Domain.Accounts.Events;

/// <summary>
/// Domain event raised when an account has been deactivated.
/// </summary>
public sealed record AccountDeactivated(
    AccountId AccountId,
    DateTimeOffset OccurredOn
) : IDomainEvent;
