using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryEventBus(IServiceProvider serviceProvider, ILogger logger)
    : InMemoryEventBus<Guid>(serviceProvider, logger);

public abstract class InMemoryEventBus<TMessageId>(IServiceProvider serviceProvider, ILogger logger)
    : IEventPublisher<TMessageId>,
      IEventSubscriber<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    // ReSharper restore MemberCanBePrivate.Global

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
        var tasks = events.Select(evt => subscriber.RunAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }

    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        var handlers = ServiceProvider.GetServices<IEventHandler<TEvent>>().ToList();
        foreach (var handler in handlers)
        {
            Logger.LogInformation("Running event handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, typeof(TEvent).FullName);
            await handler.HandleAsync(evt, cancellationToken);
        }
    }
}
