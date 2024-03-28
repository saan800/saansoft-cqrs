namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// You should never directly inherit from this class
/// use <see cref="IEvent{TId, TEntityKey}"/> instead
/// </summary>
public interface IEvent : IMessage
{
}

public interface IEvent<TId, TEntityKey> : IEvent
{
    /// <summary>
    /// Unique Id for the event
    /// This will normally be the EventStore primary key
    /// </summary>
    TId Id { get; set; }

    /// <summary>
    /// The Key of the entity that this event relates to (eg UserId, OrderId, BlogId)
    /// </summary>
    TEntityKey Key { get; set; }

    /// <summary>
    /// When the event was raised (in UTC format)
    /// </summary>
    DateTime EventOnUtc { get; set; }

    /// <summary>
    /// FullName for the type of the event
    /// </summary>
    string TypeFullName { get; set; }
}
