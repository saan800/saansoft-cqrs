namespace SaanSoft.Cqrs.Decorator.GuidIds.Messages;

public abstract class Command :
    Command<Guid>
{
    protected override Guid NewMessageId() => Guid.NewGuid();

    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TCommand, TResponse> :
    Command<Guid, TCommand, TResponse>
    where TCommand : ICommand<Guid, TCommand, TResponse>
{
    protected override Guid NewMessageId() => GuidIdGenerator.New;

    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
