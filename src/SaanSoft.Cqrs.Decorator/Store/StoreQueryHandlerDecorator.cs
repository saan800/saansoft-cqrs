using SaanSoft.Cqrs.Core.Handlers;

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreQueryHandlerDecorator<TMessageId>(IQueryHandlerRepository<TMessageId> repository, IQuerySubscriptionBus<TMessageId> next) :
    BaseStoreMessageHandlerDecorator<TMessageId>(repository),
    IQuerySubscriptionBusDecorator<TMessageId> where TMessageId : struct
{
    public async Task<TResponse> RunAsync<TQuery, TResponse>(IBaseQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        var handler = GetHandler<TQuery, TResponse>();
        var typedQuery = (TQuery)query;
        try
        {
            var response = await next.RunAsync(query, cancellationToken);
            await Repository.UpsertHandlerAsync(typedQuery.Id, handler.GetType(), null, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            await Repository.UpsertHandlerAsync(typedQuery.Id, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
        => next.GetHandler<TQuery, TResponse>();
}
