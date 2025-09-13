namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IEvent{TEntityKey}"/> instead
/// </summary>
public interface IEvent : IMessage
{
}

/// <summary>
/// Represents an event that usually alters the state of the system
/// </summary>
/// <remarks>
/// Names should be in the form of a past tense verb, eg OrderCreated, UserDetailsUpdated, BlogPostDeleted
/// </remarks>
public interface IEvent<TEntityKey> : IEvent
    where TEntityKey : struct
{
    /// <summary>
    /// The Key of the entity that this event relates to (eg UserKey, OrderKey, BlogKey)
    /// </summary>
    TEntityKey Key { get; set; }
}
