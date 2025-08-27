using System;
using Domain.Accounts.ValueObjects;
using Domain.Abstractions.Events;

namespace Domain.Accounts.Events;

/// <summary>
/// Domain event raised when an account's username has been changed.
/// </summary>
public sealed record AccountUsernameChanged(
    AccountId AccountId,
    AccountUsername OldUsername,
    AccountUsername NewUsername,
    DateTimeOffset OccurredOn
) : IDomainEvent;
