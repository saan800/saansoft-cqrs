using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class LocalEventBus(IServiceProvider serviceProvider, ILogger logger, EventBusOptions? options = null)
    : LocalEventBus<Guid>(serviceProvider, logger, options);

public abstract class LocalEventBus<TMessageId>
    : IEventBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly EventBusOptions Options;
    protected readonly LogLevel LogLevel;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger Logger;
    // ReSharper restore MemberCanBePrivate.Global

    protected LocalEventBus(IServiceProvider serviceProvider, ILogger logger, EventBusOptions? options = null)
    {
        Options = options ?? new EventBusOptions();
        LogLevel = Options.LogLevel;

        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
        => await QueueManyAsync([evt], cancellationToken);

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : IEvent<TMessageId>
    {
        var handlers = ServiceProvider.GetServices<IEventHandler<TEvent>>()?.ToList() ?? [];
        foreach (var evt in events)
        {
            foreach (var handler in handlers)
            {
                Logger.Log(LogLevel, "Running event handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, typeof(TEvent).FullName);
                await handler.HandleAsync(evt, cancellationToken);
            }
        }
    }
}
