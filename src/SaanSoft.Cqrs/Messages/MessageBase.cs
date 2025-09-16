namespace SaanSoft.Cqrs.Messages;

public abstract class MessageBase : IMessage
{
    /// <inheritdoc/>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string? CorrelationId { get; set; }

    /// <inheritdoc/>
    public string? AuthenticationId { get; set; }

    /// <inheritdoc/>
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public Guid? TriggeredByMessageId { get; set; }

    /// <inheritdoc/>
    public bool IsReplay { get; set; }

    public MessageBase()
    {
    }

    /// <summary>
    /// Copy relevant data from triggering message to a new message.
    /// - CorrelationId, AuthenticationId, TriggeredByMessageId, IsReplay
    /// </summary>
    /// <remarks>
    /// Useful when tracking a chain of messages, and want to ensure can relate them later.
    /// eg a Command might then raise Queries and Events.
    /// </remarks>
    protected MessageBase(IMessage triggeredByMessage)
    {
        CorrelationId = triggeredByMessage.CorrelationId;
        AuthenticationId = triggeredByMessage.AuthenticationId;
        TriggeredByMessageId = triggeredByMessage.Id;
        IsReplay = triggeredByMessage.IsReplay;
    }
}
