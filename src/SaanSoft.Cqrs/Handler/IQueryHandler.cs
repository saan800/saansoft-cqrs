using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Handler;

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TQuery, TResult>
    // TODO: where TResult : IQueryResult
{
    /// <summary>
    /// Handle the query and return the result, including any errors
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResult> HandleAsync(IQuery<TQuery, TResult> query, CancellationToken cancellationToken = default);
}

