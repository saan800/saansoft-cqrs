namespace SaanSoft.Cqrs.Decorator.GuidIds.Messages;

public abstract class Query<TQuery, TResponse> : Query<Guid, TQuery, TResponse>
    where TQuery : IQuery<Guid, TQuery, TResponse>
{
    protected override Guid NewMessageId() => GuidIdGenerator.New;

    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IMessage<Guid> triggeredByMessage) : base(triggeredByMessage)
    {
    }
}
