namespace SaanSoft.Cqrs.Core.Messages;

public abstract class BaseQuery<TQuery, TResult> : BaseMessage, IQuery<TQuery, TResult>
    where TQuery : IQuery<TQuery, TResult>
    where TResult : IQueryResult
{
    protected BaseQuery() : base() { }

    protected BaseQuery(IMessage message)
        : this(message.CorrelationId, message.AuthenticatedId) { }

    protected BaseQuery(string correlationId, string? authenticatedId)
        : base(correlationId, authenticatedId) { }
}
