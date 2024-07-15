namespace SaanSoft.Cqrs.Core.Handlers;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IBaseQuery<TQuery, TResponse>
{
    /// <summary>
    /// Handle the query and return the result
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
