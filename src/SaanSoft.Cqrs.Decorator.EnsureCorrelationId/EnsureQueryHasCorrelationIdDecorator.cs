namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

/// <summary>
/// Ensure that the Query has the CorrelationId field populated with a non-null and non-default value
/// </summary>
/// <param name="providers"></param>
/// <param name="next"></param>
public class EnsureQueryHasCorrelationIdDecorator(IEnumerable<ICorrelationIdProvider> providers, IQueryBus next)
    : IQueryBus
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        query.CorrelationId = providers.EnsureCorrelationId(query.CorrelationId);
        return await next.FetchAsync(query, cancellationToken);
    }
}
