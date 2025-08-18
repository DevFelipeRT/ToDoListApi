using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Domain.Abstractions.Events;
using MediatR;

namespace Infrastructure.Messaging;

public sealed class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public MediatRDomainEventDispatcher(IMediator mediator) => _mediator = mediator;

    public Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        var publishTasks = events.Select(evt =>
        {
            var wrapperType = typeof(DomainEventNotification<>).MakeGenericType(evt.GetType());
            var wrapper = (INotification)System.Activator.CreateInstance(wrapperType, evt)!;
            return _mediator.Publish(wrapper, cancellationToken);
        });

        return Task.WhenAll(publishTasks);
    }
}
