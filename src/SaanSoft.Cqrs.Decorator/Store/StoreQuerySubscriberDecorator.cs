using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreQuerySubscriberDecorator(IQuerySubscriberStore<Guid> store, IQuerySubscriber<Guid> next)
    : StoreQuerySubscriberDecorator<Guid>(store, next);

public abstract class StoreQuerySubscriberDecorator<TMessageId>(IQuerySubscriberStore<TMessageId> store, IQuerySubscriber<TMessageId> next) :
    BaseStoreMessageSubscriberDecorator<TMessageId, IQuery<TMessageId>>(store),
    IQuerySubscriber<TMessageId> where TMessageId : struct
{
    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var handler = GetHandler<TQuery, TResponse>();
        var typedQuery = (TQuery)query;
        try
        {
            var response = await next.RunAsync(query, cancellationToken);
            await Store.UpsertSubscriberAsync(typedQuery, handler.GetType(), null, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            await Store.UpsertSubscriberAsync(typedQuery, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
        => next.GetHandler<TQuery, TResponse>();
}
