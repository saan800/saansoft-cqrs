using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Handler;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TQuery, TResult>
    where TResult : IQueryResult
{
    /// <summary>
    /// Handle the query and return the result, including any errors
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public Task<TResult> HandleAsync(TQuery query);
}
