using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

/// <summary>
/// Ensure that the Query has the CorrelationId field populated with a non-null and non-default value
/// </summary>
/// <param name="providers"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class EnsureQueryHasCorrelationIdDecorator<TMessageId>(IEnumerable<ICorrelationIdProvider> providers, IBaseQueryBus<TMessageId> next)
    : IBaseQueryBus<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IBaseQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default) where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        query.Metadata.CorrelationId = providers.EnsureCorrelationId(query.Metadata.CorrelationId);
        return await next.FetchAsync(query, cancellationToken);
    }
}
