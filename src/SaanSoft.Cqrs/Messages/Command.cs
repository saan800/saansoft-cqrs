namespace SaanSoft.Cqrs.Messages;

public abstract class Command : MessageBase, ICommand
{
    public Command() : base() { }

    /// <inheritdoc/>
    protected Command(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}

public abstract class Command<TResult> : MessageBase, ICommand<TResult>
{
    public Command() : base() { }

    /// <inheritdoc/>
    protected Command(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
