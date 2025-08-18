using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Domain.Abstractions.Aggregates;
using Domain.Abstractions.Events;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    private readonly IDomainEventDispatcher _dispatcher;

    public EfUnitOfWork(ApplicationDbContext db, IDomainEventDispatcher dispatcher)
    {
        _db = db;
        _dispatcher = dispatcher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _db.SaveChangesAsync(cancellationToken);

        var domainEvents = CollectAndClearDomainEvents();

        if (domainEvents.Count > 0)
            await _dispatcher.PublishAsync(domainEvents, cancellationToken);

        return result;
    }

    private List<IDomainEvent> CollectAndClearDomainEvents()
    {
        var aggregates = _db.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToArray();

        var all = new List<IDomainEvent>(capacity: 16);
        foreach (var agg in aggregates)
            all.AddRange(agg.DequeueEvents());

        return all;
    }
}
