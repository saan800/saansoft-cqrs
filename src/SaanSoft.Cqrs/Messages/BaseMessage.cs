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

    public TMessageId? TriggeredById { get; set; }

    public string? TriggeredByUser { get; set; }

    public string? CorrelationId { get; set; }

    public DateTime MessageOnUtc { get; set; } = DateTime.UtcNow;

    public string TypeFullName { get; set; }

    public bool IsReplay { get; set; } = false;

    protected BaseMessage(TMessageId? id = null, string? correlationId = null, string? triggeredByUser = null)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        if (id.HasValue && !GenericUtils.IsNullOrDefault(id)) Id = id.Value;
        if (!string.IsNullOrWhiteSpace(correlationId)) CorrelationId = correlationId;
        if (!string.IsNullOrWhiteSpace(triggeredByUser)) TriggeredByUser = triggeredByUser;
        if (string.IsNullOrWhiteSpace(TypeFullName))
        {
            var type = GetType();
            TypeFullName ??= type.FullName ?? type.Name;
        }
    }

    protected BaseMessage(IMessage<TMessageId> triggeredByMessage)
        : this(null, triggeredByMessage.CorrelationId, triggeredByMessage.TriggeredByUser)
    {
        TriggeredById = triggeredByMessage.Id;
        IsReplay = triggeredByMessage.IsReplay;
    }
}
