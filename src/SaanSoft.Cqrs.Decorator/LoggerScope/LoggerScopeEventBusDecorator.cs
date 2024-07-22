namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the event bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
public class LoggerScopeEventBusDecorator(ILogger logger, IEventBus next) :
    IEventBus
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        using (logger.BeginScope(evt.BuildLoggingScopeData()))
        {
            logger.LogInformation("Queueing the event");
            await next.QueueAsync(evt, cancellationToken);
        }
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        var tasks = events.Select(evt => QueueAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
