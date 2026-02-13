namespace SaanSoft.Cqrs.Messages;

/// <inheritdoc cref="ICommand"/>
public abstract class Command : MessageBase, ICommand
{
    public Command() : base() { }

    /// <inheritdoc/>
    protected Command(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}

/// <inheritdoc cref="ICommand{TResponse}"/>
public abstract class Command<TResponse> : MessageBase, ICommand<TResponse>
{
    public Command() : base() { }

    /// <inheritdoc/>
    protected Command(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
