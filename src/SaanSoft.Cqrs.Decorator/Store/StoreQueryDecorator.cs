namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreQueryDecorator(IQueryRepository repository, IQueryBus next) : IQueryBus
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        await StoreMessageAsync((TQuery)query, cancellationToken);
        return await next.FetchAsync(query, cancellationToken);
    }

    private async Task StoreMessageAsync(IQuery message, CancellationToken cancellationToken)
    {
        if (!message.IsReplay)
        {
            await repository.InsertAsync(message, cancellationToken);
        }
    }
}
