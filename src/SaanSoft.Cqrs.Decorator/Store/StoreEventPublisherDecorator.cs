namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreEventPublisherDecorator<TMessageId>(IEventPublisherRepository<TMessageId> repository, IEventBus<TMessageId> next) :
    BaseStoreMessagePublisherDecorator<TMessageId, IEvent<TMessageId>>(repository),
    IEventBusDecorator<TMessageId> where TMessageId : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        await StorePublisherAsync<IEventBus<TMessageId>>(evt, cancellationToken);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        var eventList = events.ToList();
        if (eventList.Any())
        {
            await StorePublisherAsync<IEventBus<TMessageId>>(eventList.Last(), cancellationToken);
        }
        await next.QueueManyAsync(eventList, cancellationToken);
    }
}
