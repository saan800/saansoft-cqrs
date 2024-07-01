namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Because we have both Command{TMessageId} and Command{TMessageId, TCommand, TResponse} that
/// sometimes can be handled the same way, but other times need to differentiate them.
///
/// You should never directly inherit from this interface
/// use <see cref="Command{TMessageId}"/> and <see cref="Command{TMessageId, TCommand, TResponse}"/> instead.
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
public abstract class CommandRoot<TMessageId> : BaseMessage<TMessageId>, ICommandRoot<TMessageId>
    where TMessageId : struct
{
    protected CommandRoot(TMessageId? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    protected CommandRoot(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TMessageId> : CommandRoot<TMessageId>, ICommand<TMessageId>
    where TMessageId : struct
{
    protected Command(TMessageId? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    protected Command(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TMessageId, TCommand, TResponse> :
    CommandRoot<TMessageId>,
    ICommand<TMessageId, TCommand, TResponse>
    where TCommand : ICommand<TMessageId, TCommand, TResponse>
    where TMessageId : struct
{
    protected Command(TMessageId? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    protected Command(IMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}
