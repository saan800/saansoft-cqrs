using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.Core.Dispatchers;

public interface IQueryDispatcher
{
    /// <summary>
    /// Executes a query and waits for the response
    /// </summary>
    /// <param name="query"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> RunAsync<TQuery, TResult>(IQuery<TQuery, TResult> query)
        where TQuery : IQuery<TQuery, TResult>
        where TResult : IQueryResult;
}
