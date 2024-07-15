namespace SaanSoft.Cqrs.Core.Messages;

public abstract class Event<TMessageId, TEntityKey> :
    BaseMessage<TMessageId>,
    IBaseEvent<TMessageId, TEntityKey>
    where TMessageId : struct
    where TEntityKey : struct
{
    public TEntityKey Key { get; set; }

    protected Event(TEntityKey key, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        Key = key;
    }

    protected Event(TEntityKey key, IBaseMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage)
    {
        Key = key;
    }
}
