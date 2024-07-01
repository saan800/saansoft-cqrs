namespace SaanSoft.Cqrs.Messages;

public abstract class Query<TMessageId, TQuery, TResponse> :
    Query<TMessageId>,
    IQuery<TMessageId, TQuery, TResponse>
    where TMessageId : struct
    where TQuery : IQuery<TMessageId, TQuery, TResponse>
{
    protected Query(TMessageId? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId)
    {
    }

    protected Query(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
    }
}

/// <summary>
/// Base class with common properties for all query messages in standard IQuery{TMessageId} format
/// You should never directly inherit from Query{TMessageId}
///
/// Use <see cref="Query{TMessageId,TQuery,TResponse}"/> instead
/// </summary>
public abstract class Query<TMessageId> :
    BaseMessage<TMessageId>
    where TMessageId : struct
{
    protected Query(TMessageId? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId)
    {
    }

    protected Query(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
    }
}
