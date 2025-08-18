using System.Collections.Generic;
using Domain.Abstractions.Events;

namespace Domain.Abstractions.Aggregates;

/// <summary>Exposes domain events for dispatching after commit.</summary>
public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    IReadOnlyCollection<IDomainEvent> DequeueEvents();
}
