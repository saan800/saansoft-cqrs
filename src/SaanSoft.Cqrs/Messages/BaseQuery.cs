namespace SaanSoft.Cqrs.Messages;

public abstract class BaseQuery<TQuery, TResult> : BaseQuery<Guid, TQuery, TResult>
    where TQuery : IQuery<Guid, TQuery, TResult>
    where TResult : IQueryResult
{
    protected BaseQuery(string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), default, correlationId, authenticatedId) { }

    protected BaseQuery(Guid triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId) { }

    protected BaseQuery(IMessage<Guid> triggeredByMessage)
        : this(triggeredByMessage.Id, triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticatedId) { }
}

public abstract class BaseQuery<TMessageId, TQuery, TResult> : BaseMessage<TMessageId>, IQuery<TMessageId, TQuery, TResult>
    where TQuery : IQuery<TMessageId, TQuery, TResult>
    where TResult : IQueryResult
{
    protected BaseQuery(TMessageId id, TMessageId? triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId) { }

    protected BaseQuery(TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : base(id, triggeredByMessage) { }
}
