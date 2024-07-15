namespace SaanSoft.Cqrs.Core.Messages;

public abstract class BaseQuery<TMessageId, TQuery, TResponse> :
    BaseQuery<TMessageId>,
    IBaseQuery<TMessageId, TQuery, TResponse>
    where TMessageId : struct
    where TQuery : IBaseQuery<TMessageId, TQuery, TResponse>
{
    protected BaseQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected BaseQuery(IBaseMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
    }
}

/// <summary>
/// Base class with common properties for all query messages in standard IBaseQuery{TMessageId} format
/// You should never directly inherit from BaseQuery{TMessageId}
///
/// Use <see cref="BaseQuery{TMessageId,TQuery,TResponse}"/> instead
/// </summary>
public abstract class BaseQuery<TMessageId> :
    BaseMessage<TMessageId>
    where TMessageId : struct
{
    protected BaseQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected BaseQuery(IBaseMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
    }
}
