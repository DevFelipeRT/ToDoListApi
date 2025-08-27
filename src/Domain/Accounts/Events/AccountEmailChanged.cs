using System;
using Domain.Accounts.ValueObjects;
using Domain.Abstractions.Events;

namespace Domain.Accounts.Events;

/// <summary>
/// Domain event raised when an account's email has been changed.
/// </summary>
public sealed record AccountEmailChanged(
    AccountId AccountId,
    AccountEmail OldEmail,
    AccountEmail NewEmail,
    DateTimeOffset OccurredOn
) : IDomainEvent;
