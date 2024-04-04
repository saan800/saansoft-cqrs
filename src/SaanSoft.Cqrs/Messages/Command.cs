namespace SaanSoft.Cqrs.Messages;

public abstract class Command : Command<Guid>
{
    protected Command(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId) { }

    protected Command(IMessage<Guid> triggeredByMessage)
        : base(Guid.NewGuid(), triggeredByMessage) { }
}

public abstract class Command<TMessageId> : BaseMessage<TMessageId>, ICommand<TMessageId>
    where TMessageId : struct
{
    protected Command(TMessageId id, TMessageId? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId) { }

    protected Command(TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : this(id, triggeredByMessage.Id, triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticatedId) { }
}


