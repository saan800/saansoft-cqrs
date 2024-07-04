namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreQueryDecorator<TMessageId>(IQueryRepository<TMessageId> repository, IQueryBus<TMessageId> next)
    : BaseStoreMessageDecorator<TMessageId, IQuery<TMessageId>>(repository),
      IQueryBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        await StoreMessageAsync((TQuery)query, cancellationToken);
        return await next.FetchAsync(query, cancellationToken);
    }
}
