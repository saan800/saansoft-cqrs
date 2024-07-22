using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryEventBus(IServiceProvider serviceProvider)
    : IEventBus,
      IEventSubscriptionBus
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        if (GenericUtils.IsNullOrDefault(evt.Id)) evt.Id = Guid.NewGuid();

        var subscriptionBus = GetSubscriptionBus();
        await subscriptionBus.RunAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        var eventList = events.ToList();
        foreach (var evt in eventList.Where(evt => GenericUtils.IsNullOrDefault(evt.Id)))
        {
            evt.Id = Guid.NewGuid();
        }

        var subscriptionBus = GetSubscriptionBus();
        var tasks = eventList.Select(evt => subscriptionBus.RunAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Get subscription bus via ServiceProvider so it runs through any decorators
    /// </summary>
    /// <returns></returns>
    protected virtual IEventSubscriptionBus GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IEventSubscriptionBus>();

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
        => await handler.HandleAsync(evt, cancellationToken);

    public List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>()
        where TEvent : class, IEvent
        => ServiceProvider.GetPrioritisedEventHandlers<TEvent>();
}
