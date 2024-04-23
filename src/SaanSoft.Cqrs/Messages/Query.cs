namespace SaanSoft.Cqrs.Messages;

public abstract class Query<TQuery, TResponse> : Query<Guid, TQuery, TResponse>
    where TQuery : IQuery<Guid, TQuery, TResponse>
    where TResponse : IQueryResponse
{
    protected override Guid NewMessageId() => Guid.NewGuid();

    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IMessage<Guid> triggeredByMessage) : base(triggeredByMessage)
    {
    }
}

public abstract class Query<TMessageId, TQuery, TResponse> : BaseMessage<TMessageId>, IQuery<TMessageId, TQuery, TResponse>
    where TMessageId : struct
    where TQuery : IQuery<TMessageId, TQuery, TResponse>
    where TResponse : IQueryResponse
{
    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
    }
}
