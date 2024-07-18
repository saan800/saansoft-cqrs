namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the query subscription bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
public class LoggerScopeQuerySubscriptionBusDecorator(ILogger logger, IQuerySubscriptionBus next) :
    IQuerySubscriptionBusDecorator
{
    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
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
        where TQuery : class, IQuery<TQuery, TResponse>
        => next.GetHandler<TQuery, TResponse>();
}
