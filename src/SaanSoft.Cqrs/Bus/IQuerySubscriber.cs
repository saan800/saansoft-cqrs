using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IQuerySubscriber<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Run a query for information
    /// Queries will be run in replay mode.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse">
    ///     Contains the payload result of the query.
    ///     Also contains information on if the query was successful or not, and error messages.
    /// </typeparam>
    /// <returns></returns>
    Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>
        where TResponse : IQueryResponse;
}
