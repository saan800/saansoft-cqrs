using SaanSoft.Cqrs.Core.Utilities;

namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// Base class with common properties for all messages
/// You should never directly inherit from BaseMessage{TMessageId}
///
/// Use <see cref="Command{TMessageId}"/>, <see cref="Event{TMessageId,TEntityKey}"/> or <see cref="Query{TMessageId,TQuery,TResponse}"/> instead
/// </summary>
public abstract class BaseMessage<TMessageId> : IBaseMessage<TMessageId>
    where TMessageId : struct
{
    public TMessageId Id { get; set; }

    public MessageMetadata Metadata { get; set; } = new();

    public DateTime MessageOnUtc { get; set; } = DateTime.UtcNow;

    public bool IsReplay { get; set; }

    protected BaseMessage(string? correlationId = null, string? triggeredByUser = null)
    {
        if (string.IsNullOrWhiteSpace(Metadata.TypeFullName))
        {
            var type = GetType();
            Metadata.TypeFullName = type.GetTypeFullName();
        }
        if (!string.IsNullOrWhiteSpace(correlationId)) Metadata.CorrelationId = correlationId;
        if (!string.IsNullOrWhiteSpace(triggeredByUser)) Metadata.TriggeredByUser = triggeredByUser;
    }

    protected BaseMessage(IBaseMessage<TMessageId> triggeredByMessage)
        : this(triggeredByMessage.Metadata.CorrelationId, triggeredByMessage.Metadata.TriggeredByUser)
    {
        IsReplay = triggeredByMessage.IsReplay;
        Metadata.TriggeredById = triggeredByMessage.Id.ToString();
    }
}
