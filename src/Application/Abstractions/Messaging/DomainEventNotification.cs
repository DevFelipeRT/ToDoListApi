using MediatR;
using Domain.Abstractions.Events;

namespace Application.Abstractions.Messaging;

/// <summary>
/// Bridges a domain event (no external deps) to a MediatR notification.
/// This allows Application handlers to subscribe to domain events in-process.
/// </summary>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
