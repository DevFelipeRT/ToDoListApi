namespace Domain.Abstractions.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
