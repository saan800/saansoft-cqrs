namespace SaanSoft.Cqrs.Decorator.EnsureMessageId;

/// <summary>
/// Ensure that the Event has the Id field populated with a non-null and non-default value
/// </summary>
/// <param name="next"></param>
public class EnsureEventHasIdDecorator(IEventBus next)
    : IEventBusDecorator
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        if (GenericUtils.IsNullOrDefault(evt.Id)) evt.Id = Guid.NewGuid();
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        var eventList = events.ToList();
        foreach (var evt in eventList.Where(evt => GenericUtils.IsNullOrDefault(evt.Id)).ToList())
        {
            evt.Id = Guid.NewGuid();
        }
        await next.QueueManyAsync(eventList, cancellationToken);
    }
}
