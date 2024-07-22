namespace SaanSoft.Cqrs.Bus;

public interface IQuerySubscriptionBus
{
    /// <summary>
    /// Run a query for information
    /// Any issues in the query could result in an exception being thrown.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
        CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>;

    /// <summary>
    /// Get the handler for the query.
    ///
    /// Should have exactly one query handler.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : class, IQuery<TQuery, TResponse>;
}
