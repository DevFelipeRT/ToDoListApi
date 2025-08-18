using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Abstractions.Events;

namespace Application.Abstractions.Messaging;

public interface IDomainEventDispatcher
{
    Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}
