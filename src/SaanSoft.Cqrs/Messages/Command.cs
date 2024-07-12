namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Because we have both Command{TMessageId} and Command{TMessageId, TCommand, TResponse} that
/// sometimes can be handled the same way, but other times need to differentiate them.
///
/// You should never directly inherit from this interface
/// use <see cref="Command{TMessageId}"/> and <see cref="Command{TMessageId, TCommand, TResponse}"/> instead.
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
public abstract class BaseCommand<TMessageId> : BaseMessage<TMessageId>, IBaseCommand<TMessageId>
    where TMessageId : struct
{
    protected BaseCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected BaseCommand(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TMessageId> : BaseCommand<TMessageId>, ICommand<TMessageId>
    where TMessageId : struct
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TMessageId, TCommand, TResponse> :
    BaseCommand<TMessageId>,
    ICommand<TMessageId, TCommand, TResponse>
    where TCommand : ICommand<TMessageId, TCommand, TResponse>
    where TMessageId : struct
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}
