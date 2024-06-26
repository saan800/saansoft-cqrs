namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreQueryHandlerDecorator<TMessageId>(IQueryHandlerRepository<TMessageId> repository, IQuerySubscriptionBus<TMessageId> next) :
    BaseStoreMessageHandlerDecorator<TMessageId, IQuery<TMessageId>>(repository),
    IQuerySubscriptionBusDecorator<TMessageId> where TMessageId : struct
{
    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var handler = GetHandler<TQuery, TResponse>();
        var typedQuery = (TQuery)query;
        try
        {
            var response = await next.RunAsync(query, cancellationToken);
            await Repository.UpsertHandlerAsync(typedQuery, handler.GetType(), null, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            await Repository.UpsertHandlerAsync(typedQuery, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
        => next.GetHandler<TQuery, TResponse>();
}
