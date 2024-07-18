namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreEventDecorator<TEntityKey>(IEventRepository<TEntityKey> repository, IEventBus next)
    : BaseStoreMessageDecorator<IEvent>(repository),
      IEventBusDecorator
    where TEntityKey : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        await StoreMessageAsync(evt, cancellationToken);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        var eventsList = events.ToList();
        foreach (var evt in eventsList)
        {
            await StoreMessageAsync(evt, cancellationToken);
        }
        await next.QueueManyAsync(eventsList, cancellationToken);
    }
}

public class StoreEventDecorator(IEventRepository repository, IEventBus next)
    : StoreEventDecorator<Guid>(repository, next);
