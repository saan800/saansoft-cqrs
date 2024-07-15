namespace SaanSoft.Cqrs.Core.Messages;

public abstract class Query<TMessageId, TQuery, TResponse> :
    Query<TMessageId>,
    IBaseQuery<TMessageId, TQuery, TResponse>
    where TMessageId : struct
    where TQuery : IBaseQuery<TMessageId, TQuery, TResponse>
{
    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IBaseMessage<TMessageId> triggeredByMessage)
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
    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IBaseMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
    }
}
