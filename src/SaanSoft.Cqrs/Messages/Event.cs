namespace SaanSoft.Cqrs.Messages;

public abstract class Event : Event<Guid, Guid>
{
    protected override Guid NewMessageId() => Guid.NewGuid();

    protected Event(Guid key, string? correlationId = null, string? authenticatedId = null)
        : base(key, correlationId, authenticatedId) { }

    protected Event(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}

public abstract class Event<TMessageId, TEntityKey> : BaseMessage<TMessageId>, IEvent<TMessageId, TEntityKey>
    where TMessageId : struct
    where TEntityKey : struct
{
    public TEntityKey Key { get; set; }

    protected Event(TEntityKey key, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        Key = key;
    }

    protected Event(TEntityKey key, IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
        Key = key;
    }
}
