using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public interface IQueryBus<TMessageId>
{
    Task<TResult> QueryAsync<TQuery, TResult>(IQuery<TQuery, TResult> query)
        where TQuery : IQuery<TMessageId, TQuery, TResult>
        where TResult : IQueryResult;
}

