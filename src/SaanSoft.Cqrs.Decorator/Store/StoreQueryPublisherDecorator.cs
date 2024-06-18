using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreQueryPublisherDecorator(IQueryPublisherStore store, IQueryPublisher<Guid> next)
    : StoreQueryPublisherDecorator<Guid>(store, next);

public abstract class StoreQueryPublisherDecorator<TMessageId>(IQueryPublisherStore store, IQueryPublisher<TMessageId> next) :
    BaseStoreMessagePublisherDecorator(store),
    IQueryPublisher<TMessageId> where TMessageId : struct
{
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default) where TQuery : IQuery<TQuery, TResponse> where TResponse : IQueryResponse
    {
        await StorePublisher<TQuery, IQueryPublisher<TMessageId>>(cancellationToken);
        return await next.QueryAsync(query, cancellationToken);
    }
}
