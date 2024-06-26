namespace SaanSoft.Cqrs.Decorator.GuidIds.Messages;

public abstract class Event : Event<Guid, Guid>
{
    protected override Guid NewMessageId() => GuidIdGenerator.New;

    protected Event(Guid key, string? correlationId = null, string? authenticatedId = null)
        : base(key, correlationId, authenticatedId) { }

    protected Event(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
