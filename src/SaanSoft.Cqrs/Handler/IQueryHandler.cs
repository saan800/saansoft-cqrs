using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Handler;

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TQuery, TResult>
{
    Task<TResult> HandleAsync(IQuery<TQuery, TResult> query, CancellationToken cancellationToken = default);
}


// public interface IQueryHandler<in TQuery, TResult>
//     where TQuery : IQuery<TQuery, TResult>
// {
//     /// <summary>
//     /// Handle the query and return the result, including any errors
//     /// </summary>
//     /// <param name="query"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     public Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
// }
