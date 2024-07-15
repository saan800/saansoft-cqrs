using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreEventDecorator<TMessageId, TEntityKey>(IEventRepository<TMessageId, TEntityKey> repository, IBaseEventBus<TMessageId> next)
    : BaseStoreMessageDecorator<TMessageId, IBaseEvent<TMessageId>>(repository),
      IBaseEventBus<TMessageId>
    where TMessageId : struct
    where TEntityKey : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IBaseEvent<TMessageId>
    {
        await StoreMessageAsync(evt, cancellationToken);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IBaseEvent<TMessageId>
    {
        var eventsList = events.ToList();
        foreach (var evt in eventsList)
        {
            await StoreMessageAsync(evt, cancellationToken);
        }
        await next.QueueManyAsync(eventsList, cancellationToken);
    }
}
