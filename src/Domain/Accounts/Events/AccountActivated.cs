using System;
using Domain.Accounts.ValueObjects;
using Domain.Abstractions.Events;

namespace Domain.Accounts.Events;

/// <summary>
/// Domain event raised when an account has been activated.
/// </summary>
public sealed record AccountActivated(
    AccountId AccountId,
    DateTimeOffset OccurredOn
) : IDomainEvent;
