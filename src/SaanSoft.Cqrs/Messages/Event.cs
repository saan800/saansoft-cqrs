namespace SaanSoft.Cqrs.Messages;

public abstract class Event<TEntityKey> :
    BaseMessage,
    IEvent<TEntityKey>
    where TEntityKey : struct
{
    public TEntityKey Key { get; set; }

    protected Event(TEntityKey key, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        Key = key;
    }

    protected Event(TEntityKey key, IMessage triggeredByMessage)
        : base(triggeredByMessage)
    {
        Key = key;
    }
}

public abstract class Event : Event<Guid>
{
    protected Event(Guid key, string? correlationId = null, string? authenticatedId = null)
        : base(key, correlationId, authenticatedId)
    {
    }

    protected Event(Guid key, IMessage triggeredByMessage)
        : base(key, triggeredByMessage)
    {
        Key = key;
    }
}
