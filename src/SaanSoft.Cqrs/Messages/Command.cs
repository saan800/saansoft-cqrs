namespace SaanSoft.Cqrs.Messages;

public abstract class Command<TMessageId> : BaseMessage<TMessageId>, ICommand<TMessageId>
    where TMessageId : struct
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TMessageId, TCommand, TResponse> :
    BaseMessage<TMessageId>,
    ICommand<TMessageId, TCommand, TResponse>
    where TCommand : ICommand<TMessageId, TCommand, TResponse>
    where TMessageId : struct
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}
