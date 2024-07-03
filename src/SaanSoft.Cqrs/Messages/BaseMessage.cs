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
    public TMessageId Id { get; set; } = default;

    public MessageMetadata Metadata { get; set; } = new();

    public DateTime MessageOnUtc { get; set; } = DateTime.UtcNow;

    public bool IsReplay { get; set; } = false;

    protected BaseMessage(TMessageId? id = null, string? correlationId = null, string? triggeredByUser = null)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        if (id.HasValue && !GenericUtils.IsNullOrDefault(id)) Id = id.Value;

        if (string.IsNullOrWhiteSpace(Metadata.TypeFullName))
        {
            var type = GetType();
            Metadata.TypeFullName = type.FullName ?? type.Name;
        }
        if (!string.IsNullOrWhiteSpace(correlationId)) Metadata.CorrelationId = correlationId;
        if (!string.IsNullOrWhiteSpace(triggeredByUser)) Metadata.TriggeredByUser = triggeredByUser;
    }

    protected BaseMessage(IMessage<TMessageId> triggeredByMessage)
        : this(null, triggeredByMessage.Metadata.CorrelationId, triggeredByMessage.Metadata.TriggeredByUser)
    {
        IsReplay = triggeredByMessage.IsReplay;
        Metadata.TriggeredById = triggeredByMessage.Id.ToString();
    }
}
