using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Middleware;

/// <summary>
/// Non-generic envelope to make transports simpler to implement, and record metadata from the transport
/// and publisher/subscriber middleware
/// </summary>
public sealed class MessageEnvelope
{
    /// <summary>
    /// Unique identifier of the message
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Full type name of the message
    /// </summary>
    public required string MessageType { get; init; }

    /// <summary>
    /// Timestamp when the message was published in UTC
    /// </summary>
    public required DateTime OccurredOn { get; init; }

    /// <summary>
    /// The original message object
    /// </summary>
    public required object Message { get; init; }

    /// <summary>
    /// When saving messages to a database store, record the order in which they arrived.
    /// When replaying messages, should order first by <see cref="Sequence"/>, then by <see cref="OccurredOn"/>
    /// </summary>
    public long Sequence { get; init; } = 0;

    // TODO: helpers to add/overwrite/read Metadata
    /// <summary>
    /// Optional metadata
    ///
    /// Normally used by the publisher/subscriber middlewares to enrich the MessageEnvelope
    /// with additional information.
    /// </summary>
    public Dictionary<string, string> Metadata { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Where was the message published (e.g. service name, application name, etc.)
    /// </summary>
    public string? Publisher { get; init; }

    /// <summary>
    /// Status of handlers that have processed the message
    /// </summary>
    public List<HandlerRecord> Handlers { get; init; } = [];

    /// <summary>
    /// Private constructor to force use of <see cref="Wrap"/>.
    ///
    /// Also a lot of serializers (e.g. System.Text.Json, Newtonsoft.Json, etc.) and databases require a
    /// parameter-less constructor
    /// </summary>
    private MessageEnvelope()
    {
    }

    /// <summary>
    /// Create a new <see cref="MessageEnvelope"/> from the provided message
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="message"/> is null</exception>
    /// <exception cref="ArgumentException">If <paramref name="message"/> does not implement
    /// <see cref="IMessage"/></exception>
    /// <remarks>
    /// If the message does not have an Id or OccurredOn set, they will be automatically populated.
    /// </remarks>
    public static MessageEnvelope Wrap<TMessage>(TMessage message, string publisher = "") where TMessage : IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        if (message is not IMessage m) throw new ArgumentException(
            $"Message must implement {nameof(IMessage)} interface", nameof(message));

        if (m.Id.IsNullOrDefault()) m.Id = Guid.NewGuid();
        if (m.OccurredOn.IsNullOrDefault()) m.OccurredOn = DateTime.UtcNow;

        return new MessageEnvelope
        {
            Id = m.Id,
            MessageType = message.GetType().GetTypeFullName(),
            OccurredOn = m.OccurredOn,
            Message = m,
            Publisher = !string.IsNullOrWhiteSpace(publisher) ? publisher.Trim() : null
        };
    }

    /// <summary>
    /// Mark that a handler is about to process the message
    /// </summary>
    /// <remarks>
    /// Only adds a pending record if one doesn't already exist for the handler
    /// </remarks>
    public void MarkPending(string handlerName)
    {
        if (!Handlers.Any(h =>
            string.Equals(h.HandlerName, handlerName, StringComparison.OrdinalIgnoreCase) &&
            h.Status == HandlerStatus.Pending)
        )
        {
            Handlers.Add(new HandlerRecord(handlerName, HandlerStatus.Pending));
        }
    }

    /// <summary>
    /// Mark that a handler has successfully processed the message.
    ///
    /// If the handler was previously marked as pending or failed, we update the status to success and clear
    /// any error information.
    /// </summary>
    /// <remarks>
    /// Replaces any pending or failed records for the handler
    /// </remarks>
    public void MarkSuccess(string handlerName)
    {
        var existingRecords = Handlers.Where(h =>
            string.Equals(h.HandlerName, handlerName, StringComparison.OrdinalIgnoreCase) &&
            h.Status != HandlerStatus.Success
        ).ToList();

        var record = new HandlerRecord(handlerName, HandlerStatus.Success, DateTime.UtcNow);
        foreach (var h in existingRecords)
        {
            Handlers.Remove(h);
        }
        Handlers.Add(record);
    }

    /// <summary>
    /// Mark that a handler has failed to process the message
    /// </summary>
    /// <remarks>
    /// Will replace any pending records for the handler
    /// </remarks>
    public void MarkFailed(string handlerName, string errorMessage)
        => MarkFailed(handlerName, errorMessage, null);

    /// <summary>
    /// Mark that a handler has failed to process the message
    /// </summary>
    /// <remarks>
    /// Will replace any pending records for the handler
    /// </remarks>
    public void MarkFailed(string handlerName, Exception exception)
        => MarkFailed(handlerName, null, exception);

    /// <summary>
    /// Mark that a handler has failed to process the message.
    ///
    /// Must provide either an error message or an exception or both
    /// </summary>
    /// <remarks>
    /// Will replace any pending records for the handler
    /// </remarks>
    public void MarkFailed(string handlerName, string? errorMessage, Exception? exception)
    {
        if (string.IsNullOrWhiteSpace(errorMessage) && exception == null)
            throw new ArgumentException(
                $"Must provide at least one of {nameof(errorMessage)} or {nameof(exception)}");

        if (string.IsNullOrWhiteSpace(errorMessage) && exception != null)
            errorMessage = exception.Message;

        var existingPendingRecords = Handlers.Where(h =>
            string.Equals(h.HandlerName, handlerName, StringComparison.OrdinalIgnoreCase) &&
            h.Status == HandlerStatus.Pending
        ).ToList();

        var record = new HandlerRecord(handlerName, HandlerStatus.Failed, DateTime.UtcNow, errorMessage, exception);
        foreach (var h in existingPendingRecords)
        {
            Handlers.Remove(h);
        }
        Handlers.Add(record);
    }
}
