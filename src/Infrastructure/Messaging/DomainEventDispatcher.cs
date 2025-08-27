using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Abstractions.Events;
using MediatR;

namespace Infrastructure.Messaging;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator) => _mediator = mediator;

    public Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        var list = events as IList<IDomainEvent> ?? events.ToList();
        if (list.Count == 0) return Task.CompletedTask;

        var openType = typeof(DomainEventNotification<>);

        var tasks = new List<Task>(list.Count);
        foreach (var evt in list)
        {
            var notifType = openType.MakeGenericType(evt.GetType());
            var notif = (INotification)Activator.CreateInstance(notifType, evt)!;
            tasks.Add(_mediator.Publish(notif, cancellationToken));
        }

        return Task.WhenAll(tasks);
    }
}
