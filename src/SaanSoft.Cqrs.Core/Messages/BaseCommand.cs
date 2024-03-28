namespace SaanSoft.Cqrs.Core.Messages;

public abstract class BaseCommand : BaseMessage, ICommand
{
    protected BaseCommand() : base() { }

    protected BaseCommand(IMessage message)
        : this(message.CorrelationId, message.AuthenticatedId) { }

    protected BaseCommand(string correlationId, string? authenticatedId)
        : base(correlationId, authenticatedId) { }
}
