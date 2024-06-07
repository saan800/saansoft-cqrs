using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store;

namespace SaanSoft.Cqrs.Bus.Decorator;

public class StoreQueryPublisherDecorator(IQueryPublisherStore store, IQueryPublisher<Guid> next)
    : StoreQueryPublisherDecorator<Guid>(store, next);

public abstract class StoreQueryPublisherDecorator<TMessageId>(IQueryPublisherStore store, IQueryPublisher<TMessageId> next) :
    BaseMessagePublisherDecorator(store),
    IQueryPublisher<TMessageId> where TMessageId : struct
{
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default) where TQuery : IQuery<TQuery, TResponse> where TResponse : IQueryResponse
    {
        await StorePublisher<TQuery, IQueryPublisher<TMessageId>>(cancellationToken);
        return await next.QueryAsync(query, cancellationToken);
    }
}
