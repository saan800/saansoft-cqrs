namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Base class with common properties for all query messages in standard IQuery format
/// You should never directly inherit from Query
///
/// Use <see cref="Query{TQuery,TResponse}"/> instead
/// </summary>
public abstract class Query :
    BaseMessage
{
    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IMessage triggeredByMessage)
        : base(triggeredByMessage)
    {
    }
}

public abstract class Query<TQuery, TResponse> :
    Query,
    IQuery<TQuery, TResponse>
    where TQuery : IQuery<TQuery, TResponse>
{
    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IMessage triggeredByMessage)
        : base(triggeredByMessage)
    {
    }
}
