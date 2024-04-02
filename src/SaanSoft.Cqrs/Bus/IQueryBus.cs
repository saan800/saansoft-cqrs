using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IQueryBus<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Send a query for information
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResult">
    ///     Contains the payload result of the query.
    ///     Also contains information on if the query was successful or not, and error messages.
    /// </typeparam>
    /// <returns></returns>
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TMessageId, TQuery, TResult>
        where TResult : IQueryResult;
}

