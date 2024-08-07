namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreEventHandlerDecorator(IEventRepository repository, IEventSubscriptionBus next)
    : StoreEventHandlerDecorator<Guid>(repository, next);

public class StoreEventHandlerDecorator<TEntityKey>(IEventRepository<TEntityKey> repository, IEventSubscriptionBus next)
    : IEventSubscriptionBus
      where TEntityKey : struct
{
    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        // run each group of handlers in the given priority order
        foreach (var tasks in GetHandlers<TEvent>()
                     .Select(group => group.Select(handler => RunOneAsync(evt, handler, cancellationToken)))
                )
        {
            await Task.WhenAll(tasks);
        }
    }

    public async Task RunOneAsync<TEvent>(TEvent evt, IEventHandler<TEvent> handler, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        try
        {
            await next.RunOneAsync(evt, handler, cancellationToken);
            await repository.UpsertHandlerAsync(evt.Id, handler.GetType(), null, cancellationToken);
        }
        catch (Exception exception)
        {
            await repository.UpsertHandlerAsync(evt.Id, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>()
        where TEvent : class, IEvent
        => next.GetHandlers<TEvent>();
}
