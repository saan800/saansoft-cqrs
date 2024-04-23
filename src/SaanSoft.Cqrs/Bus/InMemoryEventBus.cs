using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryEventBus(IServiceProvider serviceProvider, ILogger logger, EventBusOptions? options = null)
    : InMemoryEventBus<Guid>(serviceProvider, logger, options);

public abstract class InMemoryEventBus<TMessageId>
    : IEventPublisher<TMessageId>,
      IEventSubscriber<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly EventBusOptions Options;
    protected readonly LogLevel LogLevel;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger Logger;
    // ReSharper restore MemberCanBePrivate.Global

    protected InMemoryEventBus(IServiceProvider serviceProvider, ILogger logger, EventBusOptions? options = null)
    {
        Options = options ?? new EventBusOptions();
        LogLevel = Options.LogLevel;

        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        // get subscriber via ServiceProvider so it runs through any decorators
        var subscriber = ServiceProvider.GetRequiredService<IEventSubscriber<TMessageId>>();
        await subscriber.RunAsync(evt, cancellationToken);
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        // get subscriber via ServiceProvider so it runs through any decorators
        var subscriber = ServiceProvider.GetRequiredService<IEventSubscriber<TMessageId>>();
        await subscriber.RunManyAsync(events, cancellationToken);
    }

    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
        => await RunManyAsync([evt], cancellationToken);

    public async Task RunManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) where TEvent : IEvent<TMessageId>
    {
        var handlers = ServiceProvider.GetServices<IEventHandler<TEvent>>().ToList();
        foreach (var evt in events)
        {
            foreach (var handler in handlers)
            {
                if (Logger.IsEnabled(LogLevel))
                {
                    Logger.Log(LogLevel, "Running event handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, typeof(TEvent).FullName);
                }
                await handler.HandleAsync(evt, cancellationToken);
            }
        }
    }
}
