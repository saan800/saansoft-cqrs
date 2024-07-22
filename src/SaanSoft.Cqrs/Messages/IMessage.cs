namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Base interface with common properties for all messages
/// You should never directly inherit from IMessage
/// Use <see cref="ICommand"/>, <see cref="IEvent{TEntityKey}"/> or <see cref="IQuery{TQuery, TResponse}"/> instead
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Unique Id for the command/event/query
    /// This will normally be the EventStore (or CommandStore and QueryStore if using) primary key
    /// Also used to populate the Metadata.TriggeredByMessageId property of any subsequent messages it raises
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Used to track related commands/events/queries.
    /// Should be propagated between related messages.
    ///
    /// The initial message could be populated by services such as OpenTelemetry,
    /// Http header (e.g. "X-Request-Id"), or a simple guid (as string)
    /// </summary>
    string? CorrelationId { get; set; }

    /// <summary>
    /// Who triggered the command/event/query (eg UserId, third party (eg Auth0) Id).
    ///
    /// Should be propagated between related messages.
    ///
    /// IMPORTANT: Do not use any PII data.
    /// </summary>
    string? TriggeredByUser { get; set; }

    /// <summary>
    /// FullName for the type of the message
    /// </summary>
    string? TypeFullName { get; set; }

    /// <summary>
    /// When the command/event/query was raised.
    ///
    /// When running events in order, use MessageOnUtc to run them in the correct order
    /// </summary>
    DateTime MessageOnUtc { get; set; }

    /// <summary>
    /// Whether the message is being replayed or not
    ///
    /// Note:
    /// - Events should replay
    /// - Queries should replay
    /// - Commands should NOT replay
    /// </summary>
    bool IsReplay { get; set; }

    /// <summary>
    /// Extra info mostly populated by decorators
    /// - the message itself
    /// - if this message was triggered by another message
    /// - publishing class
    /// - handlers
    /// - etc...
    /// </summary>
    MessageMetadata Metadata { get; set; }
}

