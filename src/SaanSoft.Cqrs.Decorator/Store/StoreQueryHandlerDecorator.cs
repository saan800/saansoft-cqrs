namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreQueryHandlerDecorator(IQueryRepository repository, IQuerySubscriptionBus next) :
    IQuerySubscriptionBus
{
    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        var handler = GetHandler<TQuery, TResponse>();
        var typedQuery = (TQuery)query;
        try
        {
            var response = await next.RunAsync(query, cancellationToken);
            await repository.UpsertHandlerAsync(typedQuery.Id, handler.GetType(), null, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            await repository.UpsertHandlerAsync(typedQuery.Id, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : class, IQuery<TQuery, TResponse>
        => next.GetHandler<TQuery, TResponse>();
}
