namespace SaanSoft.Cqrs.Messages;

public abstract class Command : MessageBase, ICommand
{
    public Command() : base() { }

    /// <inheritdoc/>
    protected Command(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}

public abstract class Command<TResponse> : MessageBase, ICommand<TResponse>
{
    public Command() : base() { }

    /// <inheritdoc/>
    protected Command(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
