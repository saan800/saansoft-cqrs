namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Add the publisher to the query's metadata.
///
/// Should be used in conjunction with <see cref="StoreQueryDecorator"/>
/// </summary>
/// <param name="next"></param>
public class StoreQueryPublisherDecorator(IQueryBus next) :
    BaseStoreMessagePublisherDecorator,
    IQueryBus
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        var typedQuery = (TQuery)query;
        await AddPublisherToMetadataAsync<IQueryBus>(typedQuery);
        return await next.FetchAsync(query, cancellationToken);
    }
}
