using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public abstract class Event : Event<Guid>
{
    protected Event(Guid key, Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, id, correlationId, authenticatedId) { }

    protected Event(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}

public abstract class Event<TEntityKey> :
    Event<Guid, TEntityKey>
    where TEntityKey : struct
{
    protected Event(TEntityKey key, Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, id, correlationId, authenticatedId) { }

    protected Event(TEntityKey key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
