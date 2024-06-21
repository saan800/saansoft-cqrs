using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreQueryPublisherDecorator(IQueryPublisherStore<Guid> store, IQueryPublisher<Guid> next)
    : StoreQueryPublisherDecorator<Guid>(store, next);

public abstract class StoreQueryPublisherDecorator<TMessageId>(IQueryPublisherStore<TMessageId> store, IQueryPublisher<TMessageId> next) :
    BaseStoreMessagePublisherDecorator<TMessageId, IQuery<TMessageId>>(store),
    IQueryPublisher<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var typedQuery = (TQuery)query;
        await StorePublisher<IQueryPublisher<TMessageId>>(typedQuery, cancellationToken);
        return await next.QueryAsync(query, cancellationToken);
    }
}
