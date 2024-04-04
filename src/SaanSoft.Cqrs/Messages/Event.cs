namespace SaanSoft.Cqrs.Messages;

public abstract class Event : Event<Guid, Guid>
{
    protected Event(Guid key, Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, Guid.NewGuid(), triggeredById, correlationId, authenticatedId) { }

    protected Event(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, Guid.NewGuid(), triggeredByMessage) { }
}

public abstract class Event<TMessageId, TEntityKey> : BaseMessage<TMessageId>, IEvent<TMessageId, TEntityKey>
    where TMessageId : struct
    where TEntityKey : struct
{
    public TEntityKey Key { get; set; }

    protected Event(TEntityKey key, TMessageId id, TMessageId? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId)
    {
        Key = key;
    }

    protected Event(TEntityKey key, TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : this(key, id, triggeredByMessage.Id, triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticatedId)
    {
    }
}
