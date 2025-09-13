namespace SaanSoft.Cqrs.Handlers;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handle the query and return the result.
    ///
    /// Queries should never alter any state in the system.
    /// </summary>
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}
