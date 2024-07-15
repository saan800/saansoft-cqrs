using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Add the publisher to the query's metadata.
///
/// Should be used in conjunction with <see cref="StoreQueryDecorator{TMessageId}"/>
/// </summary>
/// <param name="next"></param>
public abstract class StoreQueryPublisherDecorator<TMessageId>(IBaseQueryBus<TMessageId> next) :
    BaseStoreMessagePublisherDecorator<TMessageId>,
    IBaseQueryBus<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IBaseQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        var typedQuery = (TQuery)query;
        await StorePublisherAsync<IBaseQueryBus<TMessageId>>(typedQuery, cancellationToken);
        return await next.FetchAsync(query, cancellationToken);
    }
}
