using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreQuerySubscriberDecorator(IServiceProvider serviceProvider, IQuerySubscriberStore store, IQuerySubscriber<Guid> next)
    : StoreQuerySubscriberDecorator<Guid>(serviceProvider, store, next);

public abstract class StoreQuerySubscriberDecorator<TMessageId>(IServiceProvider serviceProvider, IQuerySubscriberStore store, IQuerySubscriber<TMessageId> next) :
    BaseStoreMessageSubscriberDecorator(serviceProvider, store),
    IQuerySubscriber<TMessageId> where TMessageId : struct
{
    protected override bool AllowMultipleSubscribers => false;

    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default) where TQuery : IQuery<TQuery, TResponse> where TResponse : IQueryResponse
    {
        await StoreSubscriber<TQuery, IQueryHandler<TQuery, TResponse>>(cancellationToken);
        return await next.RunAsync(query, cancellationToken);
    }
}
