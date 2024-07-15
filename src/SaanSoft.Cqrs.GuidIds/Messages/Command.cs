using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public abstract class Command : Command<Guid>, IMessage
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TCommand, TResponse> :
    Command<Guid, TCommand, TResponse>,
    IMessage
    where TCommand : ICommand<Guid, TCommand, TResponse>
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
