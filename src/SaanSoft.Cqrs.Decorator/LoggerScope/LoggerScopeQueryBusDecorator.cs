namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the query bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class LoggerScopeQueryBusDecorator<TMessageId>(ILogger logger, IQueryBus<TMessageId> next) :
    IQueryBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IBaseQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        var typedQuery = (TQuery)query;
        using (logger.BeginScope(typedQuery.BuildLoggingScopeData()))
        {
            logger.LogInformation("Fetching the query");
            return await next.FetchAsync(typedQuery, cancellationToken);
        }
    }
}
