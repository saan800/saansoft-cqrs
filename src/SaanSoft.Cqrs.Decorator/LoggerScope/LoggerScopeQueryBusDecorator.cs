namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the query bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
public class LoggerScopeQueryBusDecorator(ILogger logger, IQueryBus next) :
    IQueryBusDecorator
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        var typedQuery = (TQuery)query;
        using (logger.BeginScope(typedQuery.BuildLoggingScopeData()))
        {
            logger.LogInformation("Fetching the query");
            return await next.FetchAsync(typedQuery, cancellationToken);
        }
    }
}
