using System.Diagnostics.CodeAnalysis;

namespace SaanSoft.Cqrs.Messages;

public abstract class Event<TEntityKey> : MessageBase, IEvent<TEntityKey>
    where TEntityKey : struct
{
    /// <inheritdoc/>
    public required TEntityKey Key { get; set; }

    public Event() : base()
    {
    }

    protected Event(TEntityKey key) : base()
    {
        Key = key;
    }

    /// <summary>
    /// Copy relevant data from triggering message to a new message.
    /// - CorrelationId, AuthenticationId, TriggeredByMessageId, IsReplay
    /// </summary>
    /// <remarks>
    /// Useful when tracking a chain of messages, and want to ensure can relate them later.
    /// eg a Command might then raise Queries and Events.
    /// </remarks>
    protected Event(TEntityKey key, IMessage triggedByMessage) : base(triggedByMessage)
    {
        Key = key;
    }
}

/// <summary>
/// Base Event model where the entity key is a Guid
/// </summary>
public abstract class Event : Event<Guid>
{
    public Event() : base() { }

    /// <inheritdoc/>
    protected Event(Guid key) : base(key) { }

    /// <inheritdoc/>
    protected Event(Guid key, IMessage triggedByMessage) : base(key, triggedByMessage) { }
}
