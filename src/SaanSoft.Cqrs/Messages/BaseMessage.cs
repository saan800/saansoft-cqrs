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
    protected abstract TMessageId NewMessageId();

    public TMessageId Id { get; set; }
    public TMessageId? TriggeredById { get; set; }
    public string? CorrelationId { get; set; }
    public string? AuthenticationId { get; set; }
    public DateTime MessageOnUtc { get; set; } = DateTime.UtcNow;

    public string TypeFullName { get; set; }

    public bool IsReplay { get; set; } = false;

    protected BaseMessage(string? correlationId = null, string? authenticatedId = null)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        if (GenericUtils.IsNullOrDefault(Id)) Id = NewMessageId();
        if (!string.IsNullOrWhiteSpace(correlationId)) CorrelationId = correlationId;
        if (!string.IsNullOrWhiteSpace(authenticatedId)) AuthenticationId = authenticatedId;
        if (string.IsNullOrWhiteSpace(TypeFullName))
        {
            var type = GetType();
            TypeFullName ??= type.FullName ?? type.Name;
        }
    }

    protected BaseMessage(IMessage<TMessageId> triggeredByMessage)
        : this(triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticationId)
    {
        TriggeredById = triggeredByMessage.Id;
        IsReplay = triggeredByMessage.IsReplay;
    }
}
