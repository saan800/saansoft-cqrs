using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IQueryBus
{
    /// <summary>
    /// Send a query for information
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
       where TQuery : IQuery<TQuery, TResponse>;
}

// public interface IQueryBus<TMessageId> where TMessageId : struct
// {
//     /// <summary>
//     /// Send a query for information
//     /// </summary>
//     /// <param name="query"></param>
//     /// <param name="cancellationToken"></param>
//     /// <typeparam name="TQuery"></typeparam>
//     /// <typeparam name="TResult"></typeparam>
//     /// <returns></returns>
//     Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
//         where TQuery : IQuery<TQuery, TResult>;
// }

