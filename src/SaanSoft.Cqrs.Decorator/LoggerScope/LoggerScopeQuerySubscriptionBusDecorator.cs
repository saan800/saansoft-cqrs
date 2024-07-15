using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the query subscription bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class LoggerScopeQuerySubscriptionBus<TMessageId>(ILogger logger, IBaseQuerySubscriptionBus<TMessageId> next) :
    IBaseQuerySubscriptionBus<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> RunAsync<TQuery, TResponse>(IBaseQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        var handler = GetHandler<TQuery, TResponse>();
        var typedQuery = (TQuery)query;
        using (logger.BeginScope(typedQuery.BuildLoggingScopeData(handler.GetType())))
        {
            logger.LogInformation("Running query handler");
            return await next.RunAsync(typedQuery, cancellationToken);
        }
    }

    public IBaseQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
        => next.GetHandler<TQuery, TResponse>();
}
