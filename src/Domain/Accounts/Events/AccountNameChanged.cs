using System;
using Domain.Accounts.ValueObjects;
using Domain.Abstractions.Events;

namespace Domain.Accounts.Events;

/// <summary>
/// Domain event raised when an account's name has been changed.
/// </summary>
public sealed record AccountNameChanged(
    AccountId AccountId,
    AccountName OldName,
    AccountName NewName,
    DateTimeOffset OccurredOn
) : IDomainEvent;
