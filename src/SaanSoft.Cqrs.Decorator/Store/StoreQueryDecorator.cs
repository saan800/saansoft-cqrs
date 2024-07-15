using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreQueryDecorator<TMessageId>(IQueryRepository<TMessageId> repository, IBaseQueryBus<TMessageId> next)
    : BaseStoreMessageDecorator<TMessageId, IBaseQuery<TMessageId>>(repository),
      IBaseQueryBus<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IBaseQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        await StoreMessageAsync((TQuery)query, cancellationToken);
        return await next.FetchAsync(query, cancellationToken);
    }
}
