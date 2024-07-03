using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public abstract class Command : Command<Guid>
{
    protected Command(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    protected Command(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TCommand, TResponse> :
    Command<Guid, TCommand, TResponse>
    where TCommand : ICommand<Guid, TCommand, TResponse>
{
    protected Command(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    protected Command(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
