using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Add the publisher to the event's metadata.
///
/// Should be used in conjunction with <see cref="StoreEventDecorator{TMessageId, TEntityKey}"/>
/// </summary>
/// <param name="next"></param>
public abstract class StoreEventPublisherDecorator<TMessageId>(IBaseEventBus<TMessageId> next) :
    BaseStoreMessagePublisherDecorator<TMessageId>,
    IBaseEventBus<TMessageId> where TMessageId : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IBaseEvent<TMessageId>
    {
        await StorePublisherAsync<IBaseEventBus<TMessageId>>(evt, cancellationToken);
        await next.QueueAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IBaseEvent<TMessageId>
    {
        var eventList = events.ToList();
        foreach (var evt in eventList)
        {
            await StorePublisherAsync<IBaseEventBus<TMessageId>>(evt, cancellationToken);
        }
        await next.QueueManyAsync(eventList, cancellationToken);
    }
}
