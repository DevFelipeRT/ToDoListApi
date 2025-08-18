using MediatR;
using Domain.Abstractions.Events;

namespace Infrastructure.Messaging;

/// <summary>
/// Wrapper to bridge domain events (no MediatR dependency) to MediatR notifications.
/// </summary>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
