using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreEventPublisherDecorator(IEventPublisherStore<Guid> store, IEventPublisher<Guid> next)
    : StoreEventPublisherDecorator<Guid>(store, next);

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public abstract class StoreEventPublisherDecorator<TMessageId>(IEventPublisherStore<TMessageId> store, IEventPublisher<TMessageId> next) :
    BaseStoreMessagePublisherDecorator<TMessageId>(store),
    IEventPublisher<TMessageId> where TMessageId : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        await StorePublisher<TEvent, IEventPublisher<TMessageId>>(evt, cancellationToken);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        var eventList = events.ToList();
        if (eventList.Any())
        {
            await StorePublisher<TEvent, IEventPublisher<TMessageId>>(eventList.Last(), cancellationToken);
        }
        await next.QueueManyAsync(eventList, cancellationToken);
    }
}
