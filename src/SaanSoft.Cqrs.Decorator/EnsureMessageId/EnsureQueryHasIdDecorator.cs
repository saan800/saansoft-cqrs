namespace SaanSoft.Cqrs.Decorator.EnsureMessageId;

/// <summary>
/// Ensure that the Query has the Id field populated with a non-null and non-default value
/// </summary>
/// <param name="next"></param>
public class EnsureQueryHasIdDecorator(IQueryBus next)
    : IQueryBusDecorator
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        var typedQuery = (TQuery)query;
        if (GenericUtils.IsNullOrDefault(typedQuery.Id)) typedQuery.Id = Guid.NewGuid();
        return await next.FetchAsync(typedQuery, cancellationToken);
    }
}
