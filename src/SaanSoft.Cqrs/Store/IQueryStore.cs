using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Store;

/// <summary>
/// ICommandStore is primarily useful for building an audit log and/or debugging
/// Its not actually used anywhere in SaanSoft.Cqrs
/// </summary>
public interface IQueryStore<TMessageId> where TMessageId : struct
{
    /// <summary>
    /// Save the query
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task InsertAsync<TQuery>(TQuery query) where TQuery : IQuery<TMessageId>;

    /// <summary>
    /// Save queries
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task InsertManyAsync<TQuery>(IEnumerable<TQuery> query) where TQuery : IQuery<TMessageId>;
}
