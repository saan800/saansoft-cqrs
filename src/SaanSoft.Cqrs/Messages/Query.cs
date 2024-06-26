namespace SaanSoft.Cqrs.Messages;

public abstract class Query<TMessageId, TQuery, TResponse> : BaseMessage<TMessageId>, IQuery<TMessageId, TQuery, TResponse>
    where TMessageId : struct
    where TQuery : IQuery<TMessageId, TQuery, TResponse>
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
