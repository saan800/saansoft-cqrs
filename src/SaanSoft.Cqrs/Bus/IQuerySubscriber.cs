using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IQuerySubscriber<TMessageId> where TMessageId : struct
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
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>;

    /// <summary>
    /// Get the handler for the query.
    ///
    /// Should have exactly one query handler.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>;
}
