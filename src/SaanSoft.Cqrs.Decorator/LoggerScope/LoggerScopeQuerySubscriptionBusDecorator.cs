namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the query subscription bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class LoggerScopeQuerySubscriptionBusDecorator<TMessageId>(ILogger logger, IQuerySubscriptionBus<TMessageId> next) :
    IQuerySubscriptionBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var handler = GetHandler<TQuery, TResponse>();
        var typedQuery = (TQuery)query;
        using (logger.BeginScope(typedQuery.BuildLoggingScopeData(handler.GetType())))
        {
            logger.LogInformation("Running query handler");
            return await next.RunAsync(typedQuery, cancellationToken);
        }
    }

    public IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : class, IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
        => next.GetHandler<TQuery, TResponse>();
}
