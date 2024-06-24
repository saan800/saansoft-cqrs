using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreEventPublisherDecorator(IEventPublisherRepository<Guid> repository, IEventBus<Guid> next)
    : StoreEventPublisherDecorator<Guid>(repository, next);

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public abstract class StoreEventPublisherDecorator<TMessageId>(IEventPublisherRepository<TMessageId> repository, IEventBus<TMessageId> next) :
    BaseStoreMessagePublisherDecorator<TMessageId, IEvent<TMessageId>>(repository),
    IEventBusDecorator<TMessageId> where TMessageId : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        await StorePublisher<IEventBus<TMessageId>>(evt, cancellationToken);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        var eventList = events.ToList();
        if (eventList.Any())
        {
            await StorePublisher<IEventBus<TMessageId>>(eventList.Last(), cancellationToken);
        }
        await next.QueueManyAsync(eventList, cancellationToken);
    }
}
