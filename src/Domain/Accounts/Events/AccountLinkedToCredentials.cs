using System;
using Domain.Abstractions.Events;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Events
{
    /// <summary>
    /// Raised when a domain Account is linked to an external credential (IAM subject).
    /// Carries the canonical CredentialId so infra can persist the relation.
    /// </summary>
    public sealed record AccountLinkedToCredentials(
        AccountId AccountId,
        CredentialId CredentialId,
        DateTimeOffset OccurredOn) : IDomainEvent;
}