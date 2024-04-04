using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Handler;

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TQuery, TResponse>
    where TResponse : IQueryResponse
{
    /// <summary>
    /// Handle the query and return the result, including any errors
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResponse> HandleAsync(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default);
}
