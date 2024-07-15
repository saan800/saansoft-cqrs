using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public abstract class Event : Event<Guid>
{
    protected Event(Guid key, string? correlationId = null, string? authenticatedId = null)
        : base(key, correlationId, authenticatedId) { }

    protected Event(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}

public abstract class Event<TEntityKey> :
    Event<Guid, TEntityKey>,
    IMessage
    where TEntityKey : struct
{
    protected Event(TEntityKey key, string? correlationId = null, string? authenticatedId = null)
        : base(key, correlationId, authenticatedId) { }

    protected Event(TEntityKey key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
