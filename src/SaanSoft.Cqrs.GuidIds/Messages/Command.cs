using SaanSoft.Cqrs.Common.Messages;
using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public abstract class Command : BaseCommand<Guid>, IMessage
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IBaseMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}

public abstract class Command<TCommand, TResponse> :
    BaseCommand<Guid, TCommand, TResponse>,
    IMessage
    where TCommand : IBaseCommand<Guid, TCommand, TResponse>
{
    protected Command(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected Command(IBaseMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
