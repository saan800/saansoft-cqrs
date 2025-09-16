namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Base interface with common properties for all messages
/// You should never directly inherit from IMessage
/// Use <see cref="ICommand"/>, <see cref="ICommand{TResult}"/>, <see cref="IEvent{TEntityKey}"/> or
/// <see cref="IQuery{TResult}"/> instead
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Unique Id for the command/event/query message.
    /// This will normally be the EventStore (or CommandStore and QueryStore if using) primary key
    /// Also used to populate the Metadata.TriggeredByMessageId property of any subsequent messages it raises
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Used to track related commands/events/queries through the platform.
    ///
    /// Should be propagated between related messages.
    ///
    /// The initial message's CorrelationId could be populated by sources as "traceparent" header from the commonly
    /// used W3C standard, OpenTelemetry, Http header (e.g. "X-Request-Id"), or a simple guid (as string)
    /// </summary>
    string? CorrelationId { get; set; }

    /// <summary>
    /// Who triggered the command/event/query (eg User/Account Id, third party (eg Auth0) Id, Machine-2-Machine Id).
    ///
    /// Should be propagated between related messages.
    ///
    /// IMPORTANT: Do not use any PII data.
    /// </summary>
    string? AuthenticationId { get; set; }

    /// <summary>
    /// When the command/event/query was raised in UTC
    ///
    /// When running events in order, use OccurredOn to run them in the correct order
    /// </summary>
    DateTime OccurredOn { get; set; }

    /// <summary>
    /// Record if this message was triggered by another command/event/query
    /// Should be populated by the initiating command/event/query/message Id
    /// Similar to CorrelationId, it provides a way to track messages through the system
    /// </summary>
    public Guid? TriggeredByMessageId { get; set; }

    /// <summary>
    /// Whether the message is being replayed or not
    ///
    /// Note:
    /// - Events should replay
    /// - Queries should NOT replay
    /// - Commands should NOT replay
    /// </summary>
    bool IsReplay { get; set; }
}

/// <summary>
/// Base interface with common properties for messages with return types
/// You should never directly inherit from IMessage{TResult}
/// Use <see cref="ICommand{TResult}"/> or <see cref="IQuery{TResult}"/> instead
/// </summary>
public interface IMessage<TResult> : IMessage;
