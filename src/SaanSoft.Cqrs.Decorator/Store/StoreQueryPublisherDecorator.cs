using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreQueryPublisherDecorator(IQueryPublisherRepository<Guid> repository, IQueryBus<Guid> next)
    : StoreQueryPublisherDecorator<Guid>(repository, next);

public abstract class StoreQueryPublisherDecorator<TMessageId>(IQueryPublisherRepository<TMessageId> repository, IQueryBus<TMessageId> next) :
    BaseStoreMessagePublisherDecorator<TMessageId, IQuery<TMessageId>>(repository),
    IQueryBus<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var typedQuery = (TQuery)query;
        await StorePublisher<IQueryBus<TMessageId>>(typedQuery, cancellationToken);
        return await next.QueryAsync(query, cancellationToken);
    }
}
