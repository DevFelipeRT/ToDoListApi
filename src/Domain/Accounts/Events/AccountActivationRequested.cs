using System;
using Domain.Abstractions.Events;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Events
{
    /// <summary>
    /// Raised when an activation flow is requested for an account.
    /// This is a domain-level event: it signals intent to activate (e.g. an email was sent),
    /// not the presence of a cryptographic token. Handlers may initiate notifications or audits.
    /// </summary>
    public sealed record AccountActivationRequested(
        AccountId AccountId,
        CredentialId? InitiatorCredentialId,
        DateTimeOffset OccurredOn) : IDomainEvent;
}