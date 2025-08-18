// Domain/Abstractions/Aggregates/AggregateRoot.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Abstractions.Events;

namespace Domain.Abstractions.Aggregates;

/// <summary>
/// Base class for aggregate roots that raise domain events in-memory.
/// Publication is responsibility of the UnitOfWork after persistence commit.
/// </summary>
public abstract class AggregateRoot : IHasDomainEvents
{
    private readonly List<IDomainEvent> _events = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();

    protected void Raise(IDomainEvent @event)
    {
        if (@event is null) throw new ArgumentNullException(nameof(@event));
        _events.Add(@event);
    }

    public IReadOnlyCollection<IDomainEvent> DequeueEvents()
    {
        var copy = _events.ToArray();
        _events.Clear();
        return copy;
    }
}
