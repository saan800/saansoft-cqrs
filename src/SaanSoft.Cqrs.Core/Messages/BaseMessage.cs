namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// Base class with common properties for all messages
/// You should never directly inherit from BaseMessage
/// Use <see cref="BaseCommand"/>, <see cref="BaseEvent{TId, TEntityKey}"/> or <see cref="BaseQuery{TQuery, TResult}"/> insteead
/// </summary>
public abstract class BaseMessage : IMessage
{
    protected BaseMessage() { }

    protected BaseMessage(IMessage message)
        : this(message.CorrelationId, message.AuthenticatedId) { }

    protected BaseMessage(string correlationId, string? authenticatedId = null)
    {
        CorrelationId = correlationId;
        AuthenticatedId = authenticatedId;
    }

    public required string CorrelationId { get; set; }
    public string? AuthenticatedId { get; set; }
}
