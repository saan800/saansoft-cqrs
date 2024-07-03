namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreEventDecorator<TMessageId, TEntityKey>(IEventRepository<TMessageId, TEntityKey> repository, IEventBus<TMessageId> next)
    : BaseStoreMessageDecorator<TMessageId, IEvent<TMessageId>>(repository),
      IEventBusDecorator<TMessageId>
    where TMessageId : struct
    where TEntityKey : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        await StoreMessageAsync(evt, cancellationToken);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        var eventsList = events.ToList();
        foreach (var evt in eventsList)
        {
            await StoreMessageAsync(evt, cancellationToken);
        }
        await next.QueueManyAsync(eventsList, cancellationToken);
    }
}
