using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Base class with common properties for all messages
/// You should never directly inherit from BaseMessage{TMessageId}
///
/// Use <see cref="Command{TMessageId}"/>, <see cref="Event{TMessageId,TEntityKey}"/> or <see cref="Query{TMessageId,TQuery,TResponse}"/> instead
/// </summary>
public abstract class BaseMessage<TMessageId> : IMessage<TMessageId>
    where TMessageId : struct
{
    /// <summary>
    /// Unique Id for the command/event/query
    /// This will normally be the EventStore (or CommandStore and QueryStore if using) primary key
    /// Also used to populate the Metadata.TriggeredByMessageId property of any subsequent messages it raises
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Used to track related commands/events/queries.
    /// Should be propagated between related messages.
    ///
    /// The initial message could be populated by services such as OpenTelemetry,
    /// Http header (e.g. "X-Request-Id"), or a simple guid (as string)
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Who triggered the command/event/query (eg UserId, third party (eg Auth0) Id).
    ///
    /// Should be propagated between related messages.
    ///
    /// IMPORTANT: Do not use any PII data.
    /// </summary>
    public string? TriggeredByUser { get; set; }

    /// <summary>
    /// FullName for the type of the message
    /// </summary>
    public string? TypeFullName { get; set; }

    /// <summary>
    /// When the command/event/query was raised.
    ///
    /// When running events in order, use MessageOnUtc to run them in the correct order
    /// </summary>
    public DateTime MessageOnUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Extra info mostly populated by decorators
    /// - the message itself
    /// - if this message was triggered by another message
    /// - publishing class
    /// - handlers
    /// - etc...
    /// </summary>
    public MessageMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Whether the message is being replayed or not
    ///
    /// Note:
    /// - Events should replay
    /// - Queries should replay
    /// - Commands should NOT replay
    /// </summary>
    public bool IsReplay { get; set; }

    protected BaseMessage(string? correlationId = null, string? triggeredByUser = null)
    {
        if (string.IsNullOrWhiteSpace(TypeFullName)) TypeFullName = GetType().GetTypeFullName();
        if (!string.IsNullOrWhiteSpace(correlationId)) CorrelationId = correlationId;
        if (!string.IsNullOrWhiteSpace(triggeredByUser)) TriggeredByUser = triggeredByUser;
    }

    protected BaseMessage(IMessage<TMessageId> triggeredByMessage)
        : this(triggeredByMessage.CorrelationId, triggeredByMessage.TriggeredByUser)
    {
        IsReplay = triggeredByMessage.IsReplay;
        Metadata.TriggeredByMessageId = triggeredByMessage.Id.ToString();
    }
}
