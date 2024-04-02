namespace SaanSoft.Cqrs.Messages;

public abstract class BaseCommand : BaseCommand<Guid>
{
    protected BaseCommand(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId) { }

    protected BaseCommand(IMessage<Guid> triggeredByMessage)
        : base(Guid.NewGuid(), triggeredByMessage) { }
}

public abstract class BaseCommand<TMessageId> : BaseMessage<TMessageId>, ICommand<TMessageId>
    where TMessageId : struct
{
    protected BaseCommand(TMessageId id, TMessageId? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId) { }

    protected BaseCommand(TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : this(id, triggeredByMessage.Id, triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticatedId) { }
}


