namespace SaanSoft.Cqrs.Messages;

public abstract class BaseEvent : BaseEvent<Guid, Guid>
{
    protected BaseEvent(Guid key, string? correlationId = null, string? authenticatedId = null)
        : base(key, Guid.NewGuid(), default, correlationId, authenticatedId) { }

    protected BaseEvent(Guid key, Guid triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(key, Guid.NewGuid(), triggeredById, correlationId, authenticatedId) { }

    protected BaseEvent(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, Guid.NewGuid(), triggeredByMessage) { }
}

public abstract class BaseEvent<TMessageId, TEntityKey> : BaseMessage<TMessageId>, IEvent<TMessageId, TEntityKey>
{
    public TEntityKey Key { get; set; }

    protected BaseEvent(TEntityKey key, TMessageId id, TMessageId? triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId)
    {
        Key = key;
    }

    protected BaseEvent(TEntityKey key, TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : this(key, id, triggeredByMessage.Id, triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticatedId)
    {
    }
}
