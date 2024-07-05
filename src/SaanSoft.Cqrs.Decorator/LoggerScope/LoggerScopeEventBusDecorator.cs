namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the event bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class LoggerScopeEventBusDecorator<TMessageId>(ILogger logger, IEventBus<TMessageId> next) :
    IEventBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent<TMessageId>
    {
        using (logger.BeginScope(evt.BuildLoggingScopeData()))
        {
            logger.LogInformation("Queueing the event");
            await next.QueueAsync(evt, cancellationToken);
        }
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent<TMessageId>
    {
        var tasks = events.Select(evt => QueueAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
