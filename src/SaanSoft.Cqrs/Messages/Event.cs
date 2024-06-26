namespace SaanSoft.Cqrs.Messages;

public abstract class Event<TMessageId, TEntityKey> : BaseMessage<TMessageId>, IEvent<TMessageId, TEntityKey>
    where TMessageId : struct
    where TEntityKey : struct
{
    public TEntityKey Key { get; set; }

    protected Event(TEntityKey key, TMessageId? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId)
    {
        Key = key;
    }

    protected Event(TEntityKey key, IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
        Key = key;
    }
}
