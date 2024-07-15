namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IBaseEvent{TMessageId,TEntityKey}"/> instead
/// </summary>
public interface IBaseEvent : IBaseMessage
{
}

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IBaseEvent{TMessageId,TEntityKey}"/> instead
/// </summary>
public interface IBaseEvent<TMessageId> : IBaseEvent, IBaseMessage<TMessageId>
    where TMessageId : struct
{
}

public interface IBaseEvent<TMessageId, TEntityKey> : IBaseEvent<TMessageId>
    where TMessageId : struct
    where TEntityKey : struct
{
    /// <summary>
    /// The Key of the entity that this event relates to (eg UserKey, OrderKey, BlogKey)
    /// </summary>
    TEntityKey Key { get; set; }
}
