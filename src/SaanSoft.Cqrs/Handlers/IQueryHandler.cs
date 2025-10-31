namespace SaanSoft.Cqrs.Handlers;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    /// <summary>
    /// Handle the query and return the response.
    ///
    /// Queries should never alter any state in the system.
    /// </summary>
    Task<TResponse> HandleAsync(TQuery query, CancellationToken ct = default);
}
