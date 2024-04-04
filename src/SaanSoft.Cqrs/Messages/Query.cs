namespace SaanSoft.Cqrs.Messages;

public abstract class Query<TQuery, TResponse> : Query<Guid, TQuery, TResponse>
    where TQuery : IQuery<Guid, TQuery, TResponse>
    where TResponse : IQueryResponse
{
    protected Query(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId)
    {
    }

    protected Query(IMessage<Guid> triggeredByMessage) : base(Guid.NewGuid(), triggeredByMessage)
    {
    }
}

public abstract class Query<TMessageId, TQuery, TResponse> : BaseMessage<TMessageId>, IQuery<TMessageId, TQuery, TResponse>
    where TMessageId : struct
    where TQuery : IQuery<TMessageId, TQuery, TResponse>
    where TResponse : IQueryResponse
{
    protected Query(TMessageId id, TMessageId? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId)
    {
    }

    protected Query(TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : base(id, triggeredByMessage)
    {
    }
}
