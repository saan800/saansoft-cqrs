namespace SaanSoft.Cqrs.Messages;

public abstract class BaseCommand : BaseCommand<Guid>
{
    protected BaseCommand(string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), default, correlationId, authenticatedId) { }

    protected BaseCommand(Guid triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(Guid.NewGuid(), triggeredById, correlationId, authenticatedId) { }

    protected BaseCommand(IMessage<Guid> triggeredByMessage)
        : this(triggeredByMessage.Id, triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticatedId) { }
}

public abstract class BaseCommand<TMessageId> : BaseMessage<TMessageId>, ICommand<TMessageId>
{
    protected BaseCommand(TMessageId id, TMessageId? triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(id, triggeredById, correlationId, authenticatedId) { }

    protected BaseCommand(TMessageId id, IMessage<TMessageId> triggeredByMessage)
        : this(id, triggeredByMessage.Id, triggeredByMessage.CorrelationId, triggeredByMessage.AuthenticatedId) { }
}


