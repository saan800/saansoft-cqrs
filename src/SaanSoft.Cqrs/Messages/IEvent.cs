namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IEvent{TMessageId, TEntityKey}"/> instead
/// </summary>
public interface IEvent : IMessage
{
}

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IEvent{TMessageId, TEntityKey}"/> instead
/// </summary>
public interface IEvent<TMessageId> : IEvent, IMessage<TMessageId>
{
}

public interface IEvent<TMessageId, TEntityKey> : IEvent<TMessageId>
{
    /// <summary>
    /// The Key of the entity that this event relates to (eg UserId, OrderId, BlogId)
    /// </summary>
    TEntityKey Key { get; set; }
}
