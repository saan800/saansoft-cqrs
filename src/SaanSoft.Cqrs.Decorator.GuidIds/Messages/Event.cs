namespace SaanSoft.Cqrs.Decorator.GuidIds.Messages;

public abstract class Event : Event<Guid, Guid>
{
    protected Event(Guid key, Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, id, correlationId, authenticatedId) { }

    protected Event(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
