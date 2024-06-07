using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store;

namespace SaanSoft.Cqrs.Bus.Decorator;

public class StoreEventPublisherDecorator(IEventPublisherStore store, IEventPublisher<Guid> next)
    : StoreEventPublisherDecorator<Guid>(store, next);

public abstract class StoreEventPublisherDecorator<TMessageId>(IEventPublisherStore store, IEventPublisher<TMessageId> next) :
    BaseMessagePublisherDecorator(store),
    IEventPublisher<TMessageId> where TMessageId : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        await StorePublisher<TEvent, IEventPublisher<TMessageId>>(cancellationToken);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        await StorePublisher<TEvent, IEventPublisher<TMessageId>>(cancellationToken);
        await next.QueueManyAsync(events, cancellationToken);
    }
}
