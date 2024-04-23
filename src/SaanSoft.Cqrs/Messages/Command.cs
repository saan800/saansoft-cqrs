namespace SaanSoft.Cqrs.Messages;

public abstract class Command : Command<Guid>
{
    protected override Guid NewMessageId() => Guid.NewGuid();

    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TMessageId> : BaseMessage<TMessageId>, ICommand<TMessageId>
    where TMessageId : struct
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}


