namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// You should never directly inherit from this interface
/// use <see cref="IMessage{TMessageKey}"/> instead
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Used to track related commands/events/queries.
    /// Should be propagated between related messages.
    ///
    /// The initial message could be populated by services such as Azure AppInsights, OpenTelemetry,
    /// Http header (e.g. "X-Request-Id"), or a simple guid (as string)
    /// </summary>
    string? CorrelationId { get; set; }

    /// <summary>
    /// Who triggered the command/event/query (eg UserId, AuthToken)
    /// Should be propagated between related messages
    /// </summary>
    string? AuthenticatedId { get; set; }

    /// <summary>
    /// When the command/event/query was raised.
    /// When running events in order, use ReceivedOnUtc to correctly
    /// </summary>
    DateTime ReceivedOnUtc { get; set; }

    /// <summary>
    /// FullName for the type of the event
    /// </summary>
    string TypeFullName { get; set; }
}

/// <summary>
/// Base class with common properties for all messages
/// You should never directly inherit from IMessage
/// Use <see cref="ICommand{TMessageKey}"/>, <see cref="IEvent{TMessageKey, TEntityKey}"/> or <see cref="IQuery{TMessageId, TQuery, TResult}"/> instead
/// </summary>
public interface IMessage<TMessageId> : IMessage
{
    /// <summary>
    /// Unique Id for the command/event/query
    /// This will normally be the EventStore (or CommandStore and QueryStore if using) primary key
    /// Also used to populate the TriggeredById property of any subsequent messages it raises
    /// </summary>
    TMessageId Id { get; set; }

    /// <summary>
    /// Record if this message was triggered by another command/event/query
    /// Should be populated by the initiating command/event/query/message Id
    /// Similar to CorrelationId, it provides a way to track messages through the system
    /// </summary>
    TMessageId? TriggeredById { get; set; }
}
