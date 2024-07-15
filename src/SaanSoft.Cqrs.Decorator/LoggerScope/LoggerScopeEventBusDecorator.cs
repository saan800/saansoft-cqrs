using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the event bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class LoggerScopeEventBus<TMessageId>(ILogger logger, IBaseEventBus<TMessageId> next) :
    IBaseEventBus<TMessageId>
    where TMessageId : struct
{
    public async Task QueueAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IBaseEvent<TMessageId>
    {
        using (logger.BeginScope(evt.BuildLoggingScopeData()))
        {
            logger.LogInformation("Queueing the event");
            await next.QueueAsync(evt, cancellationToken);
        }
    }

    public async Task QueueManyAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
        where TEvent : class, IBaseEvent<TMessageId>
    {
        var tasks = events.Select(evt => QueueAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
