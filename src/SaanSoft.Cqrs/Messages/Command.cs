namespace SaanSoft.Cqrs.Messages;

/// <summary>
/// Because we have both Command and Command{TCommand, TResponse} that
/// sometimes can be handled the same way, but other times need to differentiate them.
///
/// You should never directly inherit from this interface
/// use <see cref="Command"/> and <see cref="Command{TCommand, TResponse}"/> instead.
/// </summary>
public abstract class BaseCommand : BaseMessage, IBaseCommand
{
    protected BaseCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected BaseCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command : BaseCommand, ICommand
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TCommand, TResponse> :
    BaseCommand,
    ICommand<TCommand, TResponse>
    where TCommand : ICommand<TCommand, TResponse>
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }
}
