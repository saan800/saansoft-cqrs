namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreQueryDecorator(IQueryRepository repository, IQueryBus next)
    : BaseStoreMessageDecorator<IQuery>(repository),
      IQueryBusDecorator
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        await StoreMessageAsync((TQuery)query, cancellationToken);
        return await next.FetchAsync(query, cancellationToken);
    }
}
