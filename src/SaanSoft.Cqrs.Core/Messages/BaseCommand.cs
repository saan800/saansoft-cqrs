namespace SaanSoft.Cqrs.Core.Messages;

/// <summary>
/// Because we have both Command{TMessageId} and Command{TMessageId, TCommand, TResponse} that
/// sometimes can be handled the same way, but other times need to differentiate them.
///
/// You should never directly inherit from this interface
/// use <see cref="BaseCommand{TMessageId}"/> and <see cref="BaseCommand{TMessageId,TCommand,TResponse}"/> instead.
/// </summary>
/// <typeparam name="TMessageId"></typeparam>
public abstract class RootCommand<TMessageId> : BaseMessage<TMessageId>, IRootCommand<TMessageId>
    where TMessageId : struct
{
    protected RootCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected RootCommand(IBaseMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class BaseCommand<TMessageId> : RootCommand<TMessageId>, IBaseCommand<TMessageId>
    where TMessageId : struct
{
    protected BaseCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected BaseCommand(IBaseMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class BaseCommand<TMessageId, TCommand, TResponse> :
    RootCommand<TMessageId>,
    IBaseCommand<TMessageId, TCommand, TResponse>
    where TCommand : IBaseCommand<TMessageId, TCommand, TResponse>
    where TMessageId : struct
{
    protected BaseCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected BaseCommand(IBaseMessage<TMessageId> triggeredByMessage)
        : base(triggeredByMessage) { }
}
