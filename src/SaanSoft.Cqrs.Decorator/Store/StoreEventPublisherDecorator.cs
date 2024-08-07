namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Add the publisher to the event's metadata.
///
/// Should be used in conjunction with <see cref="StoreEventDecorator{TEntityKey}"/>
/// </summary>
/// <param name="next"></param>
public class StoreEventPublisherDecorator(IEventBus next) :
    BaseStoreMessagePublisherDecorator,
    IEventBus
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        await AddPublisherToMetadataAsync<IEventBus>(evt);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        var eventList = events.ToList();
        foreach (var evt in eventList)
        {
            await AddPublisherToMetadataAsync<IEventBus>(evt);
        }
        await next.QueueManyAsync(eventList, cancellationToken);
    }
}
