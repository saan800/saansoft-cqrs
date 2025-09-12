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
    /// Timestamp when the message was created/published in UTC
    /// </summary>
    public required DateTime OccurredOn { get; init; }

    /// <summary>
    /// The original message object
    /// </summary>
    public required object Message { get; init; }

    /// <summary>
    /// CorrelationId / TraceId for distributed tracing
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// IdentifierCase for the user that triggered the message (e.g. user id, machine-2-machine id, etc.)
    ///
    /// This is NOT a security feature, it's just for tracking/auditing purposes.
    ///
    /// IMPORTANT: Do not include any PII or sensitive information here.
    /// </summary>
    public string? AuthenticationId { get; set; }

    /// <summary>
    /// Record if this message was triggered by another command/event/query.
    /// Should be populated by the initiating command/event/query/message Id.
    /// Similar idea to CorrelationId, it provides a way to trace messages through the system
    /// </summary>
    public string? TriggeredByMessageId { get; set; }

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
    /// parameterless constructor
    /// </summary>
    private MessageEnvelope()
    {
    }

    /// <summary>
    /// Create a new <see cref="MessageEnvelope"/> from the provided message
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="message"/> is null</exception>
    /// <exception cref="ArgumentException">If <paramref name="message"/> does not implement <see cref="IMessage"/></exception>
    /// <remarks>
    /// If the message does not have an Id or OccurredOn set, they will be automatically populated.
    /// </remarks>
    public static MessageEnvelope Wrap<TMessage>(TMessage message) where TMessage : IMessage
    {
        ArgumentNullException.ThrowIfNull(message);
        if (message is not IMessage m) throw new ArgumentException($"Message must implement {nameof(IMessage)} interface", nameof(message));

        if (m.Id.IsNullOrDefault()) m.Id = Guid.NewGuid();
        if (m.OccurredOn.IsNullOrDefault()) m.OccurredOn = DateTime.UtcNow;

        return new MessageEnvelope
        {
            Id = m.Id,
            MessageType = message.GetType().GetTypeFullName(),
            OccurredOn = m.OccurredOn,
            Message = m,
            CorrelationId = m.CorrelationId,
            AuthenticationId = m.AuthenticationId,
            TriggeredByMessageId = m.TriggeredByMessageId
        };
    }

    /// <summary>
    /// Mark that a handler is about to process the message
    /// </summary>
    public void MarkPending(string handlerName)
        => Handlers.Add(new HandlerRecord(handlerName, HandlerStatus.Pending));

    /// <summary>
    /// Mark that a handler has successfully processed the message.
    ///
    /// If the handler was previously marked as pending or failed, we update the status to success and clear
    /// any error information.
    /// </summary>
    public void MarkSuccess(string handlerName)
    {
        var index = Handlers.FindIndex(h =>
            string.Equals(h.HandlerName, handlerName, StringComparison.OrdinalIgnoreCase) &&
            h.Status != HandlerStatus.Success
        );
        if (index >= 0)
            Handlers[index] = Handlers[index] with
            {
                Status = HandlerStatus.Success,
                HandledOnUtc = DateTime.UtcNow,
                ErrorMessage = null,
                Exception = null
            };
        else
            Handlers.Add(new HandlerRecord(handlerName, HandlerStatus.Success, DateTime.UtcNow));
    }

    /// <summary>
    /// Mark that a handler has failed to process the message
    /// </summary>
    public void MarkFailed(string handlerName, string errorMessage)
        => MarkFailed(handlerName, errorMessage, null);

    /// <summary>
    /// Mark that a handler has failed to process the message
    /// </summary>
    public void MarkFailed(string handlerName, Exception exception)
        => MarkFailed(handlerName, null, exception);

    /// <summary>
    /// Mark that a handler has failed to process the message.
    ///
    /// Must provide either an error message or an exception or both
    /// </summary>
    public void MarkFailed(string handlerName, string? errorMessage, Exception? exception)
    {
        if (string.IsNullOrWhiteSpace(errorMessage) && exception == null)
            throw new ArgumentException($"Must provide at least one of {nameof(errorMessage)} or {nameof(exception)}");

        if (string.IsNullOrWhiteSpace(errorMessage) && exception != null)
            errorMessage = exception.Message;


        var index = Handlers.FindIndex(h =>
            string.Equals(h.HandlerName, handlerName, StringComparison.OrdinalIgnoreCase) &&
            h.Status == HandlerStatus.Pending
        );
        if (index >= 0)
        {
            Handlers[index] = Handlers[index] with
            {
                Status = HandlerStatus.Failed,
                HandledOnUtc = DateTime.UtcNow,
                ErrorMessage = errorMessage,
                Exception = exception
            };
        }
        else
        {
            Handlers.Add(new HandlerRecord(handlerName, HandlerStatus.Failed, DateTime.UtcNow, errorMessage, exception));
        }
    }
}
