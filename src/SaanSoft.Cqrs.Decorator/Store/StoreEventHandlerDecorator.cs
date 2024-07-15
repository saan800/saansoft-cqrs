using SaanSoft.Cqrs.Common.Handlers;
using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreEventHandlerDecorator<TMessageId>(IEventHandlerRepository<TMessageId> repository, IBaseEventSubscriptionBus<TMessageId> next)
    : BaseStoreMessageHandlerDecorator<TMessageId>(repository),
      IBaseEventSubscriptionBus<TMessageId>
    where TMessageId : struct
{
    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IBaseEvent<TMessageId>
    {
        // run each group of handlers in the given priority order
        foreach (var tasks in GetHandlers<TEvent>()
                     .Select(group => group.Select(handler => RunOneAsync(evt, handler, cancellationToken)))
                )
        {
            await Task.WhenAll(tasks);
        }
    }

    public async Task RunOneAsync<TEvent>(TEvent evt, IBaseEventHandler<TEvent> handler, CancellationToken cancellationToken = default)
        where TEvent : class, IBaseEvent<TMessageId>
    {
        try
        {
            await next.RunOneAsync(evt, handler, cancellationToken);
            await Repository.UpsertHandlerAsync(evt.Id, handler.GetType(), null, cancellationToken);
        }
        catch (Exception exception)
        {
            await Repository.UpsertHandlerAsync(evt.Id, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public List<IGrouping<int, IBaseEventHandler<TEvent>>> GetHandlers<TEvent>()
        where TEvent : class, IBaseEvent<TMessageId>
        => next.GetHandlers<TEvent>();
}
