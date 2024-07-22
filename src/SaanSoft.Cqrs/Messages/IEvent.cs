namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IEvent{TEntityKey}"/> instead
/// </summary>
public interface IEvent : IMessage
{
}

public interface IEvent<TEntityKey> : IEvent
    where TEntityKey : struct
{
    /// <summary>
    /// The Key of the entity that this event relates to (eg UserKey, OrderKey, BlogKey)
    /// </summary>
    TEntityKey Key { get; set; }
}
