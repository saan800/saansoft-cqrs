namespace SaanSoft.Cqrs.Core.Messages;

public abstract class BaseEvent<TId, TEntityKey> : BaseMessage, IEvent<TId, TEntityKey>
{
    protected BaseEvent()
    {
        if (EventOnUtc == default) EventOnUtc = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(TypeFullName))
        {
            var type = GetType();
            TypeFullName ??= type.FullName ?? type.Name;
        }
    }

    protected BaseEvent(string correlationId, string? authenticatedId)
        : this()
    {
        CorrelationId = correlationId;
        AuthenticatedId = authenticatedId;
    }

    protected BaseEvent(IMessage message)
        : this(message.CorrelationId, message.AuthenticatedId) { }

    protected BaseEvent(TEntityKey key, string correlationId, string? authenticatedId)
        : this(correlationId, authenticatedId)
    {
        Key = key;
    }

    protected BaseEvent(TId id, TEntityKey key, string correlationId, string? authenticatedId)
        : this(key, correlationId, authenticatedId)
    {
        Id = id;
    }

    public required TId Id { get; set; }
    public required TEntityKey Key { get; set; }
    public required DateTime EventOnUtc { get; set; }
    public required string TypeFullName { get; set; }
}
