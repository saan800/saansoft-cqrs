namespace SaanSoft.Cqrs.Messages;

public abstract class BaseQuery<TQuery, TResponse> : BaseQuery<Guid, TQuery, TResponse>
    where TQuery : IQuery<Guid, TQuery, TResponse>
    where TResponse : IQueryResponse
{
    protected BaseQuery(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId)
    {
    }

    protected BaseQuery(IMessage<Guid> triggeredByMessage)
        : base(Guid.NewGuid(), triggeredByMessage)
    {
    }
}

public abstract class BaseQuery<TMessageId, TQuery, TResponse> : BaseMessage<TMessageId>, IQuery<TMessageId, TQuery, TResponse>
    where TMessageId : struct
    where TQuery : IQuery<TMessageId, TQuery, TResponse>
    where TResponse : IQueryResponse
{
    protected BaseQuery(TMessageId id, TMessageId? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId)
    {
    }

    protected BaseQuery(TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : base(id, triggeredByMessage)
    {
    }
}
