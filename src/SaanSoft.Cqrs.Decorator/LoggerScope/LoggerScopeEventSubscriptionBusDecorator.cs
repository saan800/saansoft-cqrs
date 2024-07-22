namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the event subscription bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
public class LoggerScopeEventSubscriptionBusDecorator(ILogger logger, IEventSubscriptionBus next) :
    IEventSubscriptionBus
{
    public async Task RunAsync<TEvent>(TEvent evt, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
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
        using (logger.BeginScope(evt.BuildLoggingScopeData(handler.GetType())))
        {
            logger.LogInformation("Running event handler");
            await next.RunOneAsync(evt, handler, cancellationToken);
        }
    }

    public List<IGrouping<int, IEventHandler<TEvent>>> GetHandlers<TEvent>() where TEvent : class, IEvent
        => next.GetHandlers<TEvent>();
}
