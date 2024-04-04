using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Base class with common properties for all messages
/// You should never directly inherit from BaseMessage{TMessageId}
///
/// Use <see cref="BaseCommand{TMessageId}"/>, <see cref="BaseEvent{TMessageId, TEntityKey}"/> or <see cref="BaseQuery{TMessageId, TResponse}"/> instead
/// </summary>
public abstract class BaseMessage<TMessageId> : IMessage<TMessageId>
    where TMessageId : struct
{
    public TMessageId Id { get; set; }
    public TMessageId? TriggeredById { get; set; }
    public string? CorrelationId { get; set; }
    public string? AuthenticatedId { get; set; }
    public DateTime MessageOnUtc { get; set; } = DateTime.UtcNow;

    public string TypeFullName { get; set; }

    protected BaseMessage(TMessageId id, TMessageId? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
    {
        Id = id;
        if (!string.IsNullOrWhiteSpace(correlationId)) CorrelationId = correlationId;
        if (!string.IsNullOrWhiteSpace(authenticatedId)) AuthenticatedId = authenticatedId;
        if (!GenericUtils.IsNullOrDefault(triggeredById)) TriggeredById = triggeredById;
        if (string.IsNullOrWhiteSpace(TypeFullName))
        {
            var type = GetType();
            TypeFullName ??= type.FullName ?? type.Name;
        }
    }

    protected BaseMessage(TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : this(id, triggeredByMessage.Id, triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticatedId) { }

}
