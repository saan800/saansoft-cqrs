using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreEventSubscriberDecorator(IEventSubscriberStore<Guid> store, IEventSubscriber<Guid> next)
    : StoreEventSubscriberDecorator<Guid>(store, next);

public abstract class StoreEventSubscriberDecorator<TMessageId>(IEventSubscriberStore<TMessageId> store, IEventSubscriber<TMessageId> next)
    : BaseStoreMessageSubscriberDecorator<TMessageId, IEvent<TMessageId>>(store),
      IEventSubscriber<TMessageId>
    where TMessageId : struct
{
    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        // run each group of handlers in the given priority order
        foreach (var tasks in GetHandlers<TEvent>()
                     .Select(group => group.Select(handler => RunOneAsync(evt, handler, cancellationToken)))
                )
        {
            await Task.WhenAll(tasks);
        }
    }

    public async Task RunOneAsync<TEvent>(TEvent evt, IEventHandler<TEvent> handler, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        try
        {
            await next.RunOneAsync(evt, handler, cancellationToken);
            await Store.UpsertSubscriberAsync(evt, handler.GetType(), null, cancellationToken);
        }
        catch (Exception exception)
        {
            await Store.UpsertSubscriberAsync(evt, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>() where TEvent : IEvent<TMessageId>
        => next.GetHandlers<TEvent>();
}
