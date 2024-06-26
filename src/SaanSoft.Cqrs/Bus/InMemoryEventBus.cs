using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Bus;

public abstract class InMemoryEventBus<TMessageId>(IServiceProvider serviceProvider, ILogger logger)
    : IEventBus<TMessageId>,
      IEventSubscriptionBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        // get subscription bus via ServiceProvider so it runs through any decorators
        var subscriptionBus = ServiceProvider.GetRequiredService<IEventSubscriptionBus<TMessageId>>();
        await subscriptionBus.RunAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        // get subscription bus via ServiceProvider so it runs through any decorators
        var subscriptionBus = ServiceProvider.GetRequiredService<IEventSubscriptionBus<TMessageId>>();
        var tasks = events.Select(evt => subscriptionBus.RunAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
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
        Logger.LogInformation("Running event handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, evt.TypeFullName);
        await handler.HandleAsync(evt, cancellationToken);
    }

    public List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>() where TEvent : IEvent<TMessageId>
        => ServiceProvider.GetPrioritisedEventHandlers<TEvent, TMessageId>();
}
