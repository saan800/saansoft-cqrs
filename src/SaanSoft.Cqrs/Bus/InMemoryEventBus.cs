using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus;

public abstract class InMemoryEventBus<TMessageId>(IServiceProvider serviceProvider, IIdGenerator<TMessageId> idGenerator)
    : IEventBus<TMessageId>,
      IEventSubscriptionBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly IIdGenerator<TMessageId> IdGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent<TMessageId>
    {
        if (GenericUtils.IsNullOrDefault(evt.Id)) evt.Id = IdGenerator.NewId();

        var subscriptionBus = GetSubscriptionBus();
        await subscriptionBus.RunAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent<TMessageId>
    {
        var eventList = events.ToList();
        foreach (var evt in eventList.Where(evt => GenericUtils.IsNullOrDefault(evt.Id)))
        {
            evt.Id = IdGenerator.NewId();
        }

        var subscriptionBus = GetSubscriptionBus();
        var tasks = eventList.Select(evt => subscriptionBus.RunAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Get subscription bus via ServiceProvider so it runs through any decorators
    /// </summary>
    /// <returns></returns>
    protected virtual IEventSubscriptionBus<TMessageId> GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IEventSubscriptionBus<TMessageId>>();

    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent<TMessageId>
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
        where TEvent : class, IEvent<TMessageId>
        => await handler.HandleAsync(evt, cancellationToken);

    public List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>()
        where TEvent : class, IEvent<TMessageId>
        => ServiceProvider.GetPrioritisedEventHandlers<TEvent, TMessageId>();
}
