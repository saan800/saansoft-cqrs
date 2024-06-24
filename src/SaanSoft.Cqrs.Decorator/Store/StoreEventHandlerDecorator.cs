using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreEventHandlerDecorator(IEventHandlerRepository<Guid> repository, IEventSubscriptionBus<Guid> next)
    : StoreEventHandlerDecorator<Guid>(repository, next);

public abstract class StoreEventHandlerDecorator<TMessageId>(IEventHandlerRepository<TMessageId> repository, IEventSubscriptionBus<TMessageId> next)
    : BaseStoreMessageHandlerDecorator<TMessageId, IEvent<TMessageId>>(repository),
      IEventSubscriptionBus<TMessageId>
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
            await Repository.UpsertHandlerAsync(evt, handler.GetType(), null, cancellationToken);
        }
        catch (Exception exception)
        {
            await Repository.UpsertHandlerAsync(evt, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>() where TEvent : IEvent<TMessageId>
        => next.GetHandlers<TEvent>();
}
