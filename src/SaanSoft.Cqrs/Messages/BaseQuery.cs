namespace SaanSoft.Cqrs.Messages;

public abstract class BaseQuery<TQuery, TResult> : BaseQuery<Guid, TQuery, TResult>
    where TQuery : IQuery<Guid, TQuery, TResult>
    where TResult : IQueryResult
{
    protected BaseQuery(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId) { }

    protected BaseQuery(IMessage<Guid> triggeredByMessage)
        : base(Guid.NewGuid(), triggeredByMessage) { }
}

public abstract class BaseQuery<TMessageId, TQuery, TResult>
    : BaseMessage<TMessageId>, IQuery<TMessageId, TQuery, TResult>
    where TMessageId : struct
    where TQuery : IQuery<TMessageId, TQuery, TResult>
    where TResult : IQueryResult
{
    protected BaseQuery(TMessageId id, TMessageId? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId) { }

    protected BaseQuery(TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : base(id, triggeredByMessage) { }
}
